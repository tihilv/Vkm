/*
  LICENSE
  -------
  Copyright (C) 2007-2010 Ray Molenkamp

  This source code is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this source code or the software it produces.

  Permission is granted to anyone to use this source code for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this source code must not be misrepresented; you must not
     claim that you wrote the original source code.  If you use this source code
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original source code.
  3. This notice may not be removed or altered from any source distribution.
*/
/*
 * modified by hirekoke
 *   - implemented IDisposable interface
 *   - implemented GetEnumerator method
 * 
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CoreAudioApi.Interfaces;

namespace CoreAudioApi
{
    public class MMDeviceCollection : IDisposable
    {
        private IMMDeviceCollection _MMDeviceCollection;

        public int Count
        {
            get
            {
                uint result;
                Marshal.ThrowExceptionForHR(_MMDeviceCollection.GetCount(out result));
                return (int)result;
            }
        }

        public MMDevice this[int index]
        {
            get
            {
                IMMDevice result;
                _MMDeviceCollection.Item((uint)index, out result);
                return new MMDevice(result);
            }
        }

        internal MMDeviceCollection(IMMDeviceCollection parent)
        {
            _MMDeviceCollection = parent;
        }

        /* added -> */
        public IEnumerator<MMDevice> GetEnumerator()
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                MMDevice dev = this[i];
                if (dev == null) yield break;
                yield return dev;
            }
        }

        public void Dispose()
        {
            if (_MMDeviceCollection != null) Marshal.ReleaseComObject(_MMDeviceCollection);
        }
        /* <- added */
    }
}
