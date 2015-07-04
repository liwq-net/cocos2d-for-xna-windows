/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2008-2010 Ricardo Quesada
Copyright (c) 2011 Zynga Inc.


Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/
using System;
namespace cocos2d
{
    /** 
	@brief Instant actions are immediate actions. They don't have a duration like
	the CCIntervalAction actions.
	*/ 
    public class CCActionInstant : CCFiniteTimeAction
    {
        public CCActionInstant() { }

        ~CCActionInstant() { }

        public override CCObject copyWithZone(CCZone zone)
        {
            CCZone tmpZone = zone;
            CCActionInstant ret = null;

            if (tmpZone != null && tmpZone.m_pCopyObject != null)
            {
                ret = (CCActionInstant)tmpZone.m_pCopyObject;
            }
            else
            {
                ret = new CCActionInstant();
                tmpZone = new CCZone(ret);
            }

            base.copyWithZone(tmpZone);
            return ret;
        }

        public override bool isDone()
        {
            return true;
        }

        public override void step(float dt)
        {
            update(1);
        }

        public override void update(float dt)
        {
            // ignore
        }

        public override CCFiniteTimeAction reverse()
        {
            return (CCFiniteTimeAction)copy();
        }
    }
}
