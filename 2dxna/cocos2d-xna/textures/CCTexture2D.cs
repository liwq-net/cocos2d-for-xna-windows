/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (C) 2008      Apple Inc. All Rights Reserved.
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.IO;

namespace cocos2d
{
    /// <summary>
    /// Possible texture pixel formats
    /// </summary>
    public enum CCTexture2DPixelFormat
    {
        kCCTexture2DPixelFormat_Automatic = 0,
        //! 32-bit texture: RGBA8888
        kCCTexture2DPixelFormat_RGBA8888,
        //! 24-bit texture: RGBA888
        kCCTexture2DPixelFormat_RGB888,
        //! 16-bit texture without Alpha channel
        kCCTexture2DPixelFormat_RGB565,
        //! 8-bit textures used as masks
        kCCTexture2DPixelFormat_A8,
        //! 8-bit intensity texture
        kCCTexture2DPixelFormat_I8,
        //! 16-bit textures used as masks
        kCCTexture2DPixelFormat_AI88,
        //! 16-bit textures: RGBA4444
        kCCTexture2DPixelFormat_RGBA4444,
        //! 16-bit textures: RGB5A1
        kCCTexture2DPixelFormat_RGB5A1,
        //! 4-bit PVRTC-compressed texture: PVRTC4
        kCCTexture2DPixelFormat_PVRTC4,
        //! 2-bit PVRTC-compressed texture: PVRTC2
        kCCTexture2DPixelFormat_PVRTC2,

        //! Default texture format: RGBA8888
        kCCTexture2DPixelFormat_Default = kCCTexture2DPixelFormat_RGBA8888,

        // backward compatibility stuff
        kTexture2DPixelFormat_Automatic = kCCTexture2DPixelFormat_Automatic,
        kTexture2DPixelFormat_RGBA8888 = kCCTexture2DPixelFormat_RGBA8888,
        kTexture2DPixelFormat_RGB888 = kCCTexture2DPixelFormat_RGB888,
        kTexture2DPixelFormat_RGB565 = kCCTexture2DPixelFormat_RGB565,
        kTexture2DPixelFormat_A8 = kCCTexture2DPixelFormat_A8,
        kTexture2DPixelFormat_RGBA4444 = kCCTexture2DPixelFormat_RGBA4444,
        kTexture2DPixelFormat_RGB5A1 = kCCTexture2DPixelFormat_RGB5A1,
        kTexture2DPixelFormat_Default = kCCTexture2DPixelFormat_Default
    } ;

    /// <summary>
    /// Extension to set the Min / Mag filter
    /// </summary>
    public struct ccTexParams
    {
        public uint minFilter;
        public uint magFilter;
        public uint wrapS;
        public uint wrapT;
    }

    /// <summary>
    /// This class allows to easily create OpenGL 2D textures from images, text or raw data.
    /// The created CCTexture2D object will always have power-of-two dimensions. 
    /// Depending on how you create the CCTexture2D object, the actual image area of the texture might be smaller than the texture dimensions i.e. "contentSize" != (pixelsWide, pixelsHigh) and (maxS, maxT) != (1.0, 1.0).
    /// Be aware that the content of the generated textures will be upside-down!
    /// </summary>
    public class CCTexture2D : CCObject
    {
        // If the image has alpha, you can create RGBA8 (32-bit) or RGBA4 (16-bit) or RGB5A1 (16-bit)
        // Default is: RGBA8888 (32-bit textures)
        public static CCTexture2DPixelFormat g_defaultAlphaPixelFormat = CCTexture2DPixelFormat.kCCTexture2DPixelFormat_Default;
        private static Color g_MyBlack = new Color(0, 0, 0, 0);
        private static uint NameIndex = 1;

        public CCTexture2D()
        {
            m_uPixelsWide = 0;
            m_uPixelsHigh = 0;
            m_fMaxS = 0.0f;
            m_fMaxT = 0.0f;
#if ANDROID
            m_bHasPremultipliedAlpha = false;
            m_bPVRHaveAlphaPremultiplied = false;
#else
            m_bHasPremultipliedAlpha = false;
            m_bPVRHaveAlphaPremultiplied = true;
#endif
            m_tContentSize = new CCSize();
            Name = NameIndex++;
        }

        public Texture2D Texture
        {
            get
            {
                return (texture2D);
            }
            set
            {
                texture2D = value;
            }
        }

        public string description()
        {
            string ret = "<CCTexture2D | Dimensions = " + m_uPixelsWide + " x " + m_uPixelsHigh + " | Coordinates = (" + m_fMaxS + ", " + m_fMaxT + ")>";
            return ret;
        }

        #region raw data

        /// <summary>
        /// These functions are needed to create mutable textures
        /// </summary>
        public void releaseData(object data)
        {
            // throw new NotImplementedException();
        }

        /*public object keepData(object data, uint length)
        {
            throw new NotImplementedException();
        }
         */

        /// <summary>
        /// Intializes with a texture2d with data
        /// </summary>
        public bool initWithData(object data, CCTexture2DPixelFormat pixelFormat, uint pixelsWide, uint pixelsHigh, CCSize contentSize)
        {
            SurfaceFormat format = SurfaceFormat.Color;
            switch(pixelFormat) {
                case CCTexture2DPixelFormat.kCCTexture2DPixelFormat_A8:
                    format = SurfaceFormat.Alpha8;
                    break;
                case CCTexture2DPixelFormat.kCCTexture2DPixelFormat_AI88:
                    break;
                case CCTexture2DPixelFormat.kCCTexture2DPixelFormat_Automatic:
                    format = SurfaceFormat.Color;
                    break;
                case CCTexture2DPixelFormat.kCCTexture2DPixelFormat_Default:
                    format = SurfaceFormat.Color;
                    break;
                case CCTexture2DPixelFormat.kCCTexture2DPixelFormat_I8:
                    format = SurfaceFormat.Single;
                    break;
                case CCTexture2DPixelFormat.kCCTexture2DPixelFormat_PVRTC2:
                    format = SurfaceFormat.Vector2;
                    break;
                case CCTexture2DPixelFormat.kCCTexture2DPixelFormat_PVRTC4:
                    format = SurfaceFormat.Vector4;
                    break;
                case CCTexture2DPixelFormat.kCCTexture2DPixelFormat_RGB565:
                    format = SurfaceFormat.Bgr565;
                    break;
                case CCTexture2DPixelFormat.kCCTexture2DPixelFormat_RGB5A1:
                    format = SurfaceFormat.Bgra5551;
                    break;
                case CCTexture2DPixelFormat.kCCTexture2DPixelFormat_RGB888:
                    format = SurfaceFormat.Color;
                    break;
                case CCTexture2DPixelFormat.kCCTexture2DPixelFormat_RGBA4444:
                    format = SurfaceFormat.Bgra4444;
                    break;
            }
            Texture2D t = new Texture2D(CCApplication.sharedApplication().GraphicsDevice, (int)pixelsWide, (int)pixelsHigh, false, format);
            // This was the old version that is not compatible with non-WP7 devices.
            //            Texture2D t = new Texture2D(CCApplication.sharedApplication().GraphicsDevice, (int)contentSize.width, (int)contentSize.height, false, format);
            if (data is byte[])
            {
                t.SetData((byte[])data);
            }
            m_tContentSize = new CCSize(contentSize);
            m_uPixelsWide = (int)pixelsWide;
            m_uPixelsHigh = (int)pixelsHigh;
            m_ePixelFormat = pixelFormat;
            m_fMaxS = contentSize.width / (float)(pixelsWide);
            m_fMaxT = contentSize.height / (float)(pixelsHigh);

            m_bHasPremultipliedAlpha = false;
            /*
            long POTWide, POTHigh;
            POTWide = ccUtils.ccNextPOT(texture.Width);
            POTHigh = ccUtils.ccNextPOT(texture.Height);

            int maxTextureSize = CCConfiguration.sharedConfiguration().MaxTextureSize;
            if (POTHigh > maxTextureSize || POTWide > maxTextureSize)
            {
                CCLog.Log(string.Format("cocos2d: WARNING: Image ({0} x {1}) is bigger than the supported {2} x {3}", POTWide, POTHigh, maxTextureSize, maxTextureSize));
                return false;
            }*/

            texture2D = t;
            // return initWithTexture(t);
            return (true);
        }

        #endregion

        #region create extensions

        /**
        Extensions to make it easy to create a CCTexture2D object from a string of text.
        Note that the generated textures are of type A8 - use the blending mode (GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA).
        */

        public bool initWithString(string text, CCSize dimensions, CCTextAlignment alignment, string fontName, float fontSize)
        {
            return (initWithString(text, dimensions, alignment, fontName, fontSize, Color.YellowGreen, g_MyBlack));
        }

        ///<summary>
        /// Initializes a texture from a string with dimensions, alignment, font name and font size
        /// </summary>
        public bool initWithString(string text, CCSize dimensions, CCTextAlignment alignment, string fontName, float fontSize, Color fgColor, Color bgColor)
        {
            if (dimensions.width < 0 || dimensions.height < 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            SpriteFont font = null;
            try
            {
                font = CCApplication.sharedApplication().content.Load<SpriteFont>(@"fonts/" + fontName);
            }
            catch (Exception)
            {
                if (fontName.EndsWith(".spritefont", StringComparison.OrdinalIgnoreCase))
                {
                    fontName = fontName.Substring(0, fontName.Length - 11);
                    font = CCApplication.sharedApplication().content.Load<SpriteFont>(@"fonts/" + fontName);
                }
            }
            if (CCSize.CCSizeEqualToSize(dimensions, new CCSize()))
            {
                Vector2 temp = font.MeasureString(text);
                dimensions.width = temp.X;
                dimensions.height = temp.Y;
            }

            Vector2 origin;
            if (CCTextAlignment.CCTextAlignmentRight == alignment)
            {
                origin = new Vector2(-(dimensions.width - font.MeasureString(text).X), 0);
            }
            else if (CCTextAlignment.CCTextAlignmentCenter == alignment)
            {
                origin = new Vector2(-(dimensions.width - font.MeasureString(text).X) / 2.0f, 0);
            }
            else
            {
                origin = new Vector2(0, 0);
            }

            float scale = 1.0f;//need refer fontSize;
            try
            {
            CCApplication app = CCApplication.sharedApplication();

            //*  for render to texture
            RenderTarget2D renderTarget = new RenderTarget2D(app.graphics.GraphicsDevice, (int)dimensions.width, (int)dimensions.height);
            app.graphics.GraphicsDevice.SetRenderTarget(renderTarget);
                app.graphics.GraphicsDevice.Clear(bgColor);

            app.spriteBatch.Begin();
                app.spriteBatch.DrawString(font, text, new Vector2(0, 0), fgColor, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
            app.spriteBatch.End();

            app.graphics.GraphicsDevice.SetRenderTarget(null);

            // to copy the rendered target data to a plain texture(to the memory)
            Color[] colors1D = new Color[renderTarget.Width * renderTarget.Height];
            renderTarget.GetData(colors1D);
            texture2D = new Texture2D(app.GraphicsDevice, renderTarget.Width, renderTarget.Height);
            texture2D.SetData(colors1D);

            return initWithTexture(texture2D);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return (false);
        }

        public bool initWithString(string text, string fontName, float fontSize, Color fgColor, Color bgColor)
        {
            return initWithString(text, new CCSize(0, 0), CCTextAlignment.CCTextAlignmentCenter, fontName, fontSize, fgColor, bgColor);
        }
        /// <summary>
        ///  Initializes a texture from a string with font name and font size
        /// </summary>
        public bool initWithString(string text, string fontName, float fontSize)
        {
            return initWithString(text, new CCSize(0, 0), CCTextAlignment.CCTextAlignmentCenter, fontName, fontSize);
        }

#if CC_SUPPORT_PVRTC	
	    /**
	    Extensions to make it easy to create a CCTexture2D object from a PVRTC file
	    Note that the generated textures don't have their alpha premultiplied - use the blending mode (GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA).
	    */
	    /** Initializes a texture from a PVRTC buffer */
        public bool initWithPVRTCData(object data, int level, int bpp, bool hasAlpha, int length, CCTexture2DPixelFormat pixelFormat)
        {
            throw new NotImplementedException();
        }
#endif // CC_SUPPORT_PVRTC

        /** Initializes a texture from a PVR file */
        public bool initWithPVRFile(string file)
        {
            throw new NotImplementedException();
        }

        public bool initWithTexture(Texture2D texture, CCSize contentSize)
        {
            bool result = initWithTexture(texture);
            ContentSizeInPixels = contentSize;
            return (result);
        }
        /// <summary>
        /// Initializes a texture from a content file
        /// </summary>
        public bool initWithTexture(Texture2D texture)
        {
            if (null == texture)
            {
                return false;
            }

            long POTWide, POTHigh;
            POTWide = ccUtils.ccNextPOT(texture.Width);
            POTHigh = ccUtils.ccNextPOT(texture.Height);

            int maxTextureSize = CCConfiguration.sharedConfiguration().MaxTextureSize;
            if (POTHigh > maxTextureSize || POTWide > maxTextureSize)
            {
                CCLog.Log(string.Format("cocos2d: WARNING: Image ({0} x {1}) is bigger than the supported {2} x {3}", POTWide, POTHigh, maxTextureSize, maxTextureSize));
                return false;
            }
#if ANDROID
            return initTextureWithImage(texture, texture.Width, texture.Height);
#else
            return initPremultipliedATextureWithImage(texture, texture.Width, texture.Height);
#endif
        }

        public bool initTextureWithImage(Texture2D texture, int POTWide, int POTHigh)
        {
            texture2D = texture;
            m_tContentSize.width = texture2D.Width;
            m_tContentSize.height = texture2D.Height;

            m_uPixelsWide = POTWide;
            m_uPixelsHigh = POTHigh;
            //m_ePixelFormat = pixelFormat;
            m_fMaxS = m_tContentSize.width / (float)(POTWide);
            m_fMaxT = m_tContentSize.height / (float)(POTHigh);

            return true;
        }

        public bool initPremultipliedATextureWithImage(Texture2D texture, int POTWide, int POTHigh)
        {
#warning "to set pixelFormat,alpha"
            initTextureWithImage(texture, POTWide, POTHigh);
            m_bHasPremultipliedAlpha = true;
            return true;
        }

        /** Initializes a texture from a file */
        public bool initWithFile(string file)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void SaveAsJpeg(Stream stream, int width, int height)
        {
            if (this.texture2D != null)
            {
#if WINDOWS || XBOX || WP7
                this.texture2D.SaveAsJpeg(stream, width, height);
#endif
            }
        }

        public void SaveAsPng(Stream stream, int width, int height)
        {
            if (this.texture2D != null)
            {
#if WINDOWS || XBOX || WP7
                this.texture2D.SaveAsPng(stream, width, height);
#endif
            }
        }

        /** sets the min filter, mag filter, wrap s and wrap t texture parameters.
        If the texture size is NPOT (non power of 2), then in can only use GL_CLAMP_TO_EDGE in GL_TEXTURE_WRAP_{S,T}.
        @since v0.8
        */
        public void setTexParameters(ccTexParams texParams)
        {
            //throw new NotImplementedException();
        }

        /** sets antialias texture parameters:
        - GL_TEXTURE_MIN_FILTER = GL_LINEAR
        - GL_TEXTURE_MAG_FILTER = GL_LINEAR

        @since v0.8
        */
        public void setAntiAliasTexParameters()
        {
            // ccTexParams texParams = { GL_LINEAR, GL_LINEAR, GL_CLAMP_TO_EDGE, GL_CLAMP_TO_EDGE };
            // this->setTexParameters(&texParams);
        }

        /** sets alias texture parameters:
        - GL_TEXTURE_MIN_FILTER = GL_NEAREST
        - GL_TEXTURE_MAG_FILTER = GL_NEAREST

        @since v0.8
        */
        public void setAliasTexParameters()
        {
            ccTexParams texParams = new ccTexParams();// { GL_NEAREST, GL_NEAREST, GL_CLAMP_TO_EDGE, GL_CLAMP_TO_EDGE };
            this.setTexParameters(texParams);
        }


        /** Generates mipmap images for the texture.
        It only works if the texture size is POT (power of 2).
        @since v0.99.0
        */
        public void generateMipmap()
        {


#warning SpriteTest
            // throw new NotImplementedException();
        }

        /** returns the bits-per-pixel of the in-memory OpenGL texture
        @since v1.0
        */
        public uint bitsPerPixelForFormat()
        {
            throw new NotImplementedException();
        }

        public void setPVRImagesHavePremultipliedAlpha(bool haveAlphaPremultiplied)
        {
            m_bPVRHaveAlphaPremultiplied = haveAlphaPremultiplied;
        }

        /** sets the default pixel format for UIImages that contains alpha channel.
        If the UIImage contains alpha channel, then the options are:
        - generate 32-bit textures: kCCTexture2DPixelFormat_RGBA8888 (default one)
        - generate 24-bit textures: kCCTexture2DPixelFormat_RGB888
        - generate 16-bit textures: kCCTexture2DPixelFormat_RGBA4444
        - generate 16-bit textures: kCCTexture2DPixelFormat_RGB5A1
        - generate 16-bit textures: kCCTexture2DPixelFormat_RGB565
        - generate 8-bit textures: kCCTexture2DPixelFormat_A8 (only use it if you use just 1 color)

        How does it work ?
        - If the image is an RGBA (with Alpha) then the default pixel format will be used (it can be a 8-bit, 16-bit or 32-bit texture)
        - If the image is an RGB (without Alpha) then an RGB565 or RGB888 texture will be used (16-bit texture)

        @since v0.8
        */
        static public void setDefaultAlphaPixelFormat(CCTexture2DPixelFormat format)
        {
            throw new NotImplementedException();
        }

        /** returns the alpha pixel format
        @since v0.8
        */
        static public CCTexture2DPixelFormat defaultAlphaPixelFormat()
        {
            throw new NotImplementedException();
        }

        //private bool initPremultipliedATextureWithImage(CCImage image, uint pixelsWide, uint pixelsHigh)
        //{
        //    throw new NotImplementedException();
        //}

        // By default PVR images are treated as if they don't have the alpha channel premultiplied
        private bool m_bPVRHaveAlphaPremultiplied;

        #region Property

        private Texture2D texture2D;
        public Texture2D getTexture2D()
        {
            return texture2D;
        }

        private CCTexture2DPixelFormat m_ePixelFormat;
        /// <summary>
        /// pixel format of the texture
        /// </summary>
        public CCTexture2DPixelFormat PixelFormat
        {
            get { return m_ePixelFormat; }
            set { m_ePixelFormat = value; }
        }

        private int m_uPixelsWide;
        /// <summary>
        /// width in pixels
        /// </summary>
        public int PixelsWide
        {
            get { return m_uPixelsWide; }
            set { m_uPixelsWide = value; }
        }

        private int m_uPixelsHigh;
        /// <summary>
        /// hight in pixels
        /// </summary>
        public int PixelsHigh
        {
            get { return m_uPixelsHigh; }
            set { m_uPixelsHigh = value; }
        }

        private uint m_uName;
        /// <summary>
        /// texture name
        /// </summary>
        public uint Name
        {
            get { return m_uName; }
            set { m_uName = value; }
        }

        private CCSize m_tContentSize;
        /// <summary>
        /// content size in pixels, which can be overridden by the code that uses the
        /// frame grabber to grab this texture.
        /// </summary>
        public CCSize ContentSizeInPixels
        {
            get { return m_tContentSize; }
            set { m_tContentSize = value; }
        }

        /// <summary>
        /// returns the content size of the texture in points
        /// </summary>
        public CCSize getContentSize()
        {
            CCSize ret = new CCSize();
            ret.width = m_tContentSize.width / ccMacros.CC_CONTENT_SCALE_FACTOR();
            ret.height = m_tContentSize.height / ccMacros.CC_CONTENT_SCALE_FACTOR();

            return ret;
        }

        private float m_fMaxS;
        /// <summary>
        /// texture max S
        /// </summary>
        public float MaxS
        {
            get { return m_fMaxS; }
            set { m_fMaxS = value; }
        }

        private float m_fMaxT;
        /// <summary>
        /// texture max T
        /// </summary>
        public float MaxT
        {
            get { return m_fMaxT; }
            set { m_fMaxT = value; }
        }

        private bool m_bHasPremultipliedAlpha;
        /// <summary>
        /// whether or not the texture has their Alpha premultiplied
        /// </summary>
        public bool HasPremultipliedAlpha
        {
            get { return m_bHasPremultipliedAlpha; }
            set { m_bHasPremultipliedAlpha = value; }
        }

        #endregion
    }
}
