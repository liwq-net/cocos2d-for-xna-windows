/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2008-2010 Ricardo Quesada
Copyright (c) 2011 Zynga Inc.
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
    /// <summary>
    /// @brief CCTransitionTurnOffTiles:
    /// Turn off the tiles of the outgoing scene in random order
    /// </summary>
    public class CCTransitionTurnOffTiles : CCTransitionScene, ICCTransitionEaseScene
    {
        public override void onEnter()
        {
            base.onEnter();
            CCSize s = CCDirector.sharedDirector().getWinSize();
            float aspect = s.width / s.height;
            int x = (int)(12 * aspect);
            int y = 12;

            CCTurnOffTiles toff = CCTurnOffTiles.actionWithSize(new ccGridSize(x, y), m_fDuration);
            CCFiniteTimeAction action = easeActionWithAction(toff);
            m_pOutScene.runAction
            (
                CCSequence.actions
                (
                    action,
                    CCCallFunc.actionWithTarget(this, (base.finish)),
                    CCStopGrid.action()
                )
            );
        }

        public virtual CCFiniteTimeAction easeActionWithAction(CCActionInterval action)
        {
            return action;
        }

        //DECLEAR_TRANSITIONWITHDURATION(CCTransitionTurnOffTiles);
        public new static CCTransitionTurnOffTiles transitionWithDuration(float t, CCScene scene)
        {
            CCTransitionTurnOffTiles pScene = new CCTransitionTurnOffTiles();
            if (pScene.initWithDuration(t, scene))
            {
                return pScene;
            }

            return null;
        }

        protected override void sceneOrder()
        {
            m_bIsInSceneOnTop = false;
        }
    }
}
