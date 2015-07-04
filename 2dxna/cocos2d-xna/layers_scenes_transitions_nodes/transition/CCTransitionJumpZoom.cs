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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cocos2d
{
    public class CCTransitionJumpZoom : CCTransitionScene
    {
        public override void onEnter()
        {
            base.onEnter();
            CCSize s = CCDirector.sharedDirector().getWinSize();

            m_pInScene.scale = 0.5f;
            m_pInScene.position = new CCPoint(s.width, 0);
            m_pInScene.anchorPoint = new CCPoint(0.5f, 0.5f);
            m_pOutScene.anchorPoint = new CCPoint(0.5f, 0.5f);

            CCActionInterval jump = CCJumpBy.actionWithDuration(m_fDuration / 4, new CCPoint(-s.width, 0), s.width / 4, 2);
            CCActionInterval scaleIn = CCScaleTo.actionWithDuration(m_fDuration / 4, 1.0f);
            CCActionInterval scaleOut = CCScaleTo.actionWithDuration(m_fDuration / 4, 0.5f);

            CCActionInterval jumpZoomOut = (CCActionInterval)(CCSequence.actions(scaleOut, jump));
            CCActionInterval jumpZoomIn = (CCActionInterval)(CCSequence.actions(jump, scaleIn));

            CCActionInterval delay = CCDelayTime.actionWithDuration(m_fDuration / 2);

            m_pOutScene.runAction(jumpZoomOut);
            m_pInScene.runAction
            (
                CCSequence.actions
                (
                    delay,
                    jumpZoomIn,
                    CCCallFunc.actionWithTarget(this, base.finish)
                )
            );
        }

        //public DECLEAR_TRANSITIONWITHDURATION(CCTransitionJumpZoom);
        public static new CCTransitionJumpZoom transitionWithDuration(float t, CCScene scene)
        {
            CCTransitionJumpZoom pScene = new CCTransitionJumpZoom();
            if (pScene != null && pScene.initWithDuration(t, scene))
            {
                return pScene;
            }
            pScene = null;
            return null;
        }
    }
}
