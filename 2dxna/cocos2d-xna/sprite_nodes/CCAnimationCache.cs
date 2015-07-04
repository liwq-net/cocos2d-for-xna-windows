﻿/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2008-2011 Ricardo Quesada
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
using System.Collections.Generic;
using System.Diagnostics;

namespace cocos2d
{
    /** Singleton that manages the Animations.
    It saves in a cache the animations. You should use this class if you want to save your animations in a cache.

    Before v0.99.5, the recommend way was to save them on the CCSprite. Since v0.99.5, you should use this class instead.

    @since v0.99.5
    */
    public class CCAnimationCache : CCObject
    {
        public CCAnimationCache()
        {
        }

        ~CCAnimationCache()
        { 
        }
		
		/** Retruns ths shared instance of the Animation cache */
        public static CCAnimationCache sharedAnimationCache()
        {
            if (null == s_pSharedAnimationCache)
            {
                s_pSharedAnimationCache = new CCAnimationCache();
                s_pSharedAnimationCache.init();
            }

            return s_pSharedAnimationCache;
        }

		/** Purges the cache. It releases all the CCAnimation objects and the shared instance.
		*/
        public static void purgeSharedAnimationCache()
        {
            //CC_SAFE_RELEASE_NULL(s_pSharedAnimationCache);
            s_pSharedAnimationCache = null;
        }

		/** Adds a CCAnimation with a name.
		*/
        public void addAnimation(CCAnimation animation, string name)
        { 
            m_pAnimations.Add(name, animation);
        }

		/** Deletes a CCAnimation from the cache.
		*/
        public void removeAnimationByName(string name)
        {
            if (null == name)
            {
                return;
            }

            m_pAnimations.Remove(name);
        }

		/** Returns a CCAnimation that was previously added.
		If the name is not found it will return nil.
		You should retain the returned copy if you are going to use it.
		*/
        public CCAnimation animationByName(string name)
        { 
            CCAnimation animation = new CCAnimation();
            if (m_pAnimations.TryGetValue(name, out animation))
            {
                return animation;
            }
            else
            {
                return null;
            }
        }

        public bool init()
        { 
            m_pAnimations = new Dictionary<string, CCAnimation>();
		    return true;
        }

		Dictionary<string, CCAnimation> m_pAnimations;
		static CCAnimationCache s_pSharedAnimationCache;


    }
}