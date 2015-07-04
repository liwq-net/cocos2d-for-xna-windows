/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2008-2010 Ricardo Quesada
Copyright (c) 2011      Zynga Inc.
Copyright (c) 2011-2012 openxlive.com

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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cocos2d
{
    public enum ccSceneFlag
    {
        ccNormalScene = 1 << 0,
        ccTransitionScene = 1 << 1
    }

    /// <summary>
    /// brief CCScene is a subclass of CCNode that is used only as an abstract concept.
    /// CCScene an CCNode are almost identical with the difference that CCScene has it's
    /// anchor point (by default) at the center of the screen.
    /// For the moment CCScene has no other logic than that, but in future releases it might have
    /// additional logic.
    ///  It is a good practice to use and CCScene as the parent of all your nodes.
    /// </summary>
    public class CCScene : CCNode
    {
        protected ccSceneFlag m_eSceneType;
        public ccSceneFlag SceneType
        {
            get { return m_eSceneType; }
        }

        public CCScene()
        {
            isRelativeAnchorPoint = false;
            anchorPoint = CCPointExtension.ccp(0.5f, 0.5f);
            m_eSceneType = ccSceneFlag.ccNormalScene;
        }

        public bool init()
        {
            bool bRet = false;
            do
            {
                CCDirector director = CCDirector.sharedDirector();
                if (director == null)
                {
                    break;
                }

                contentSize = director.getWinSize();
                // success
                bRet = true;
            } while (false);
            return bRet;
        }

        public static new CCScene node()
        {
            CCScene pRet = new CCScene();
            if (pRet.init())
            {
                return pRet;
            }
            else
            {
                return null;
            }
        }
    }

    public class CCNormalScene : CCScene
    {
        public CCNormalScene()
        {
        }

        ~CCNormalScene()
        {
        }

        public static new CCNormalScene node()
        {
            CCNormalScene pRet = new CCNormalScene();
            if (pRet.init())
            {
                return pRet;
            }
            else
            {
                return null;
            }
        }
    }
}
