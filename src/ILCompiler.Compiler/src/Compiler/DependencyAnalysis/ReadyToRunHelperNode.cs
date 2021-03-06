﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILCompiler.DependencyAnalysisFramework;
using Internal.TypeSystem;

namespace ILCompiler.DependencyAnalysis
{
    public enum ReadyToRunHelperId
    {
        NewHelper,
        NewArr1,
        VirtualCall,
        IsInstanceOf,
        CastClass,
        GetNonGCStaticBase,
        GetGCStaticBase,
        GetThreadStaticBase,
        DelegateCtor,
    }

    public partial class ReadyToRunHelperNode : AssemblyStubNode
    {
        private ReadyToRunHelperId _id;
        private Object _target;

        public ReadyToRunHelperNode(ReadyToRunHelperId id, Object target)
        {
            _id = id;
            _target = target;
        }

        public override string GetName()
        {
            return ((ISymbolNode)this).MangledName;
        }

        public ReadyToRunHelperId Id
        {
            get
            {
                return _id;
            }
        }

        public Object Target
        {
            get
            {
                return _target;
            }
        }

        public override string MangledName
        {
            get
            {
                switch (_id)
                {
                    case ReadyToRunHelperId.NewHelper:
                        return "__NewHelper_" + NodeFactory.NameMangler.GetMangledTypeName((TypeDesc)_target);
                    case ReadyToRunHelperId.NewArr1:
                        return "__NewArr1_" + NodeFactory.NameMangler.GetMangledTypeName((TypeDesc)_target);
                    case ReadyToRunHelperId.VirtualCall:
                        return "__VirtualCall_" + NodeFactory.NameMangler.GetMangledMethodName((MethodDesc)_target);
                    case ReadyToRunHelperId.IsInstanceOf:
                        return "__IsInstanceOf_" + NodeFactory.NameMangler.GetMangledTypeName((TypeDesc)_target);
                    case ReadyToRunHelperId.CastClass:
                        return "__CastClass_" + NodeFactory.NameMangler.GetMangledTypeName((TypeDesc)_target);
                    case ReadyToRunHelperId.GetNonGCStaticBase:
                        return "__GetNonGCStaticBase_" + NodeFactory.NameMangler.GetMangledTypeName((TypeDesc)_target);
                    case ReadyToRunHelperId.GetGCStaticBase:
                        return "__GetGCStaticBase_" + NodeFactory.NameMangler.GetMangledTypeName((TypeDesc)_target);
                    case ReadyToRunHelperId.GetThreadStaticBase:
                        return "__GetThreadStaticBase_" + NodeFactory.NameMangler.GetMangledTypeName((TypeDesc)_target);
                    case ReadyToRunHelperId.DelegateCtor:
                        return "__DelegateCtor_" + NodeFactory.NameMangler.GetMangledMethodName(((DelegateInfo)_target).Target);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected override DependencyList ComputeNonRelocationBasedDependencies(NodeFactory context)
        {
            if (_id == ReadyToRunHelperId.VirtualCall)
            {
                DependencyList dependencyList = new DependencyList();
                dependencyList.Add(context.VirtualMethodUse((MethodDesc)_target), "ReadyToRun Virtual Method Call");
                return dependencyList;
            }
            else
            {
                return null;
            }
        }
    }
}
