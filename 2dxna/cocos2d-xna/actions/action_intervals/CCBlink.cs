﻿/****************************************************************************
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
    /** @brief Blinks a CCNode object by modifying it's visible attribute
    */
    public class CCBlink : CCActionInterval
    {
        public static CCBlink actionWithDuration(float duration, uint uBlinks)
        {
	        CCBlink pBlink = new CCBlink();
	        pBlink.initWithDuration(duration, uBlinks);

	        return pBlink;
        }

        public bool initWithDuration(float duration, uint uBlinks)
        {
	        if (base.initWithDuration(duration))
	        {
                m_nTimes = uBlinks;
		        return true;
	        }

	        return false;
        }

        public override CCObject copyWithZone(CCZone pZone)
        {
	        CCZone pNewZone = null;
	        CCBlink pCopy = null;
	        if(pZone != null && pZone.m_pCopyObject != null) 
	        {
		        //in case of being called at sub class
		        pCopy = (CCBlink)(pZone.m_pCopyObject);
	        }
	        else
	        {
		        pCopy = new CCBlink();
		        pZone = pNewZone = new CCZone(pCopy);
	        }

	        base.copyWithZone(pZone);

	        pCopy.initWithDuration(m_fDuration, m_nTimes);
	
	        return pCopy;
        }

        public override void update(float time)
        {
	        if (m_pTarget != null && ! isDone())
	        {
		        float slice = 1.0f / m_nTimes;
		        // float m = fmodf(time, slice);
                float m = time % slice;
		        m_pTarget.visible = m > slice / 2 ? true : false;
	        }
        }

        public override CCFiniteTimeAction reverse()
        {
	        return CCBlink.actionWithDuration(m_fDuration, m_nTimes);
        }

        protected uint m_nTimes;
    }
}