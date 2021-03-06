// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include <stdio.h>
#include <stdlib.h>
#include <fcntl.h>
#include <unistd.h>
#include <assert.h>
#include <errno.h>
#include "pal_utilities.h"

extern "C" int32_t Write2(int32_t fd, const void* buffer, int32_t bufferSize)
{
    assert(buffer != nullptr || bufferSize == 0);
    assert(bufferSize >= 0);

    if (bufferSize < 0)
    {
        errno = ERANGE;
        return -1;
    }

    ssize_t count = write(fd, buffer, UnsignedCast(bufferSize));
    assert(count >= -1 && count <= bufferSize);
    return static_cast<int32_t>(count);
}

