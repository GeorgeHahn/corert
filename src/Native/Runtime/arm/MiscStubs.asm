;;
;; Copyright (c) Microsoft. All rights reserved.
;; Licensed under the MIT license. See LICENSE file in the project root for full license information. 
;;

#include "AsmMacros.h"

    EXTERN RhpShutdownHelper
    EXTERN GetClasslibCCtorCheck
    EXTERN memcpy
    EXTERN memcpyGCRefs

    TEXTAREA

;;
;; Currently called only from a managed executable once Main returns, this routine does whatever is needed to
;; cleanup managed state before exiting.
;;
;;  Input:
;;      r0 : Process exit code
;;
    NESTED_ENTRY RhpShutdown

        COOP_PINVOKE_FRAME_PROLOG

        ;; Call the bulk of the helper implemented in C++. Takes the exit code already in r0.
        bl      RhpShutdownHelper

        COOP_PINVOKE_FRAME_EPILOG

    NESTED_END RhpShutdown

;;
;; Checks whether the static class constructor for the type indicated by the context structure has been
;; executed yet. If not the classlib is called via their CheckStaticClassConstruction callback which will
;; execute the cctor and update the context to record this fact.
;;
;;  Input:
;;      r0 : Address of StaticClassConstructionContext structure
;;
;;  Output:
;;      All volatile registers and the condition codes may be trashed.
;;
    LEAF_ENTRY RhpCheckCctor

        ;; Check the m_initialized field of the context. The cctor has been run only if this equals 1 (the
        ;; initial state is 0 and the remaining values are reserved for classlib use). This check is
        ;; unsynchronized; if we go down the slow path and call the classlib then it is responsible for
        ;; synchronizing with other threads and re-checking the value.
        ldr     r12, [r0, #OFFSETOF__StaticClassConstructionContext__m_initialized]
        cmp     r12, #1
        bne     RhpCheckCctor__SlowPath
        bx      lr
RhpCheckCctor__SlowPath
        mov     r1, r0
        b       RhpCheckCctor2 ; tail-call the check cctor helper that actually has an implementation to call
                               ; the cctor
    LEAF_END RhpCheckCctor

;;
;; Checks whether the static class constructor for the type indicated by the context structure has been
;; executed yet. If not the classlib is called via their CheckStaticClassConstruction callback which will
;; execute the cctor and update the context to record this fact.
;;
;;  Input:
;;      r0 : Value that must be preserved in this register across the cctor check.
;;      r1 : Address of StaticClassConstructionContext structure
;;
;;  Output:
;;      All volatile registers other than r0 may be trashed and the condition codes may also be trashed.
;;
    LEAF_ENTRY RhpCheckCctor2

        ;; Check the m_initialized field of the context. The cctor has been run only if this equals 1 (the
        ;; initial state is 0 and the remaining values are reserved for classlib use). This check is
        ;; unsynchronized; if we go down the slow path and call the classlib then it is responsible for
        ;; synchronizing with other threads and re-checking the value.
        ldr     r12, [r1, #OFFSETOF__StaticClassConstructionContext__m_initialized]
        cmp     r12, #1
        bne     RhpCheckCctor2__SlowPath
        bx      lr

    LEAF_END RhpCheckCctor2

;;
;; Slow path helper for RhpCheckCctor.
;;
;;  Input:
;;      r0 : Value that must be preserved in this register across the cctor check.
;;      r1 : Address of StaticClassConstructionContext structure
;;
;;  Output:
;;      All volatile registers other than r0 may be trashed and the condition codes may also be trashed.
;;
    NESTED_ENTRY RhpCheckCctor2__SlowPath

        ;; Need to preserve r0, r1 and lr across helper call. r2 is also pushed to keep the stack 8 byte aligned.
        PROLOG_PUSH {r0-r2,lr}

        ;; Call a C++ helper to retrieve the address of the classlib callback. The caller's return address is
        ;; passed as the argument to the helper; it's an address in the module and is used by the helper to
        ;; locate the classlib.
        mov     r0, lr
        bl      GetClasslibCCtorCheck

        ;; R0 now contains the address of the classlib method to call. The single argument is the context
        ;; structure address currently in stashed on the stack. Clean up and tail call to the classlib
        ;; callback so we're not on the stack should a GC occur (so we don't need to worry about transition
        ;; frames).
        mov     r12, r0
        EPILOG_POP {r0-r2,lr}
        ;; tail-call the class lib cctor check function. This function is required to return its first
        ;; argument, so that r0 can be preserved.
        EPILOG_BRANCH_REG r12

    NESTED_END RhpCheckCctor__SlowPath2

;;
;; Input:
;;      r0: address of location on stack containing return address.
;;      
;; Outpt:
;;      r0: proper (unhijacked) return address
;;
;; Trashes: r1, r2
;;
    LEAF_ENTRY RhpLoadReturnAddress

        INLINE_GETTHREAD   r1, r2
        ldr     r2, [r1, #OFFSETOF__Thread__m_ppvHijackedReturnAddressLocation]
        cmp     r0, r2
        beq     GetHijackedReturnAddress
        ldr     r0, [r0]
        bx      lr

GetHijackedReturnAddress
        ldr     r0, [r1, #OFFSETOF__Thread__m_pvHijackedReturnAddress]
        bx      lr

    LEAF_END RhpLoadReturnAddress


    ;;
    ;; r0 = output buffer (an IntPtr[] managed object)
    ;;
    NESTED_ENTRY RhGetCurrentThreadStackTrace

        COOP_PINVOKE_FRAME_PROLOG

        ;; pass-through argument registers
        bl          RhpCalculateStackTraceWorker

        nop     ; debugger bug workaround, this fixes the stack trace

        COOP_PINVOKE_FRAME_EPILOG

    NESTED_END RhGetCurrentThreadStackTrace

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;; void* RhpCopyMultibyteNoGCRefs(void*, void*, size_t)
;;
;; The purpose of this wrapper is to hoist the potential null reference exceptions of copying memory up to a place where
;; the stack unwinder and exception dispatch can properly transform the exception into a managed exception and dispatch
;; it to managed code.
;;

    LEAF_ENTRY    RhpCopyMultibyteNoGCRefs

        ; r0    dest
        ; r1    src
        ; r2    count

        cmp     r2, #0                      ; check for a zero-length copy
        beq     NothingToCopy_NoGCRefs

        ; Now check the dest and src pointers.  If they AV, the EH subsystem will recognize the address of the AV,
        ; unwind the frame, and fixup the stack to make it look like the (managed) caller AV'ed, which will be 
        ; translated to a managed exception as usual.
    ALTERNATE_ENTRY RhpCopyMultibyteNoGCRefsDestAVLocation
        ldrb    r3, [r0]
    ALTERNATE_ENTRY RhpCopyMultibyteNoGCRefsSrcAVLocation
        ldrb    r3, [r1]

        ; tail-call to plain-old-memcpy
        b       memcpy

NothingToCopy_NoGCRefs
        ; dest is already still in r0
        bx      lr

    LEAF_END


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;; void* RhpCopyMultibyte(void*, void*, size_t)
;;
;; The purpose of this wrapper is to hoist the potential null reference exceptions of copying memory up to a place where
;; the stack unwinder and exception dispatch can properly transform the exception into a managed exception and dispatch
;; it to managed code.
;;

    LEAF_ENTRY    RhpCopyMultibyte

        ; r0    dest
        ; r1    src
        ; r2    count

        cmp     r2, #0                      ; check for a zero-length copy
        beq     NothingToCopy

        ; Now check the dest and src pointers.  If they AV, the EH subsystem will recognize the address of the AV,
        ; unwind the frame, and fixup the stack to make it look like the (managed) caller AV'ed, which will be 
        ; translated to a managed exception as usual.
    ALTERNATE_ENTRY RhpCopyMultibyteDestAVLocation
        ldrb    r3, [r0]
    ALTERNATE_ENTRY RhpCopyMultibyteSrcAVLocation
        ldrb    r3, [r1]

        ; tail-call to the GC-safe memcpy implementation
        b       memcpyGCRefs

NothingToCopy
        ; dest is already still in r0
        bx      lr

    LEAF_END



    end
