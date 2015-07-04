using System;
using cocos2d;
using Microsoft.Xna.Framework;

namespace Cocos2DGame1
{
    public class IntroLayer : CCLayerColor
    {
        public IntroLayer()
        {

            //// create and initialize a Label
            //var label = new CCLabelTTF("Hello Cocos2D-XNA", "MarkerFelt", 22);

            //// position the label on the center of the screen
            //label.Position = CCDirector.SharedDirector.WinSize.Center;

            //// add the label as a child to this Layer
            //AddChild(label);

            //// setup our color for the background
            //Color = new CCColor3B(Microsoft.Xna.Framework.Color.Blue);
            //Opacity = 255;

            CCLayerColor layer = new CCLayerColor();
            layer.initWithColorWidthHeight(new ccColor4B(0xff, 0xff, 0, 0xff), 100, 100);
            this.addChild(layer);

            CCParticleSun sun = new CCParticleSun();
            this.addChild(sun);

        }

        public static CCScene Scene
        {
            get
            {
                // 'scene' is an autorelease object.
                var scene = new CCScene();

                // 'layer' is an autorelease object.
                var layer = new IntroLayer();

                // add layer as a child to scene
                scene.addChild(layer);

                // return the scene
                return scene;

            }

        }

    }
}

