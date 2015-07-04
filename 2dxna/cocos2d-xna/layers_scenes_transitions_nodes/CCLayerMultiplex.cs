using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cocos2d
{
    /// <summary>
    /// CCMultipleLayer is a CCLayer with the ability to multiplex it's children.
    /// Features:
    /// - It supports one or more children
    /// - Only one children will be active a time
    /// </summary>
    public class CCLayerMultiplex : CCLayer
    {
        protected uint m_nEnabledLayer;
        protected List<CCLayer> m_pLayers;

        public CCLayerMultiplex()
        {

        }

        /// <summary>
        ///  creates a CCLayerMultiplex with one or more layers using a variable argument list. 
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static CCLayerMultiplex layerWithLayers(params CCLayer[] layer)
        {
            CCLayerMultiplex pMultiplexLayer = new CCLayerMultiplex();
            if (pMultiplexLayer != null && pMultiplexLayer.initWithLayers(layer))
            {
                //pMultiplexLayer->autorelease();
                return pMultiplexLayer;
            }
            pMultiplexLayer = null;
            return null;
        }

        /// <summary>
        ///  * lua script can not init with undetermined number of variables
        /// * so add these functinons to be used with lua.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static CCLayerMultiplex layerWithLayer(CCLayer layer)
        {
            CCLayerMultiplex pMultiplexLayer = new CCLayerMultiplex();
            pMultiplexLayer.initWithLayer(layer);
            //pMultiplexLayer->autorelease();
            return pMultiplexLayer;
        }

        public virtual void addLayer(CCLayer layer)
        {
            Debug.Assert(m_pLayers != null);
            m_pLayers.Add(layer);
        }

        public virtual bool initWithLayer(CCLayer layer)
        {
            m_pLayers = new List<CCLayer>(1);
            m_pLayers.Add(layer);
            m_nEnabledLayer = 0;
            this.addChild(layer);
            return true;
        }

        /** initializes a MultiplexLayer with one or more layers using a variable argument list. */
        public virtual bool initWithLayers(params CCLayer[] layer)
        {
            m_pLayers = new List<CCLayer>(5);
            //m_pLayers->retain();

            m_pLayers.AddRange(layer);
            //for (int i = 0; i < layer.Length; i++)
            //{
            //    m_pLayers.Add(layer[i]);
            //}
            m_nEnabledLayer = 0;
            this.addChild(m_pLayers[(int)m_nEnabledLayer]);
            return true;
        }
        /** switches to a certain layer indexed by n. 
        The current (old) layer will be removed from it's parent with 'cleanup:YES'.
        */
        public virtual void switchTo(uint n)
        {
            if (n >= m_pLayers.Count)
            {
                CCLog.Log("Invalid index in MultiplexLayer switchTo message");
                return;
            }
            this.removeChild(m_pLayers[(int)m_nEnabledLayer], false);
            m_nEnabledLayer = n;
            this.addChild(m_pLayers[(int)n]);
        }
        /** release the current layer and switches to another layer indexed by n.
        The current (old) layer will be removed from it's parent with 'cleanup:YES'.
        */
        public virtual void switchToAndReleaseMe(uint n)
        {
            if (n >= m_pLayers.Count)
            {
                CCLog.Log("Invalid index in MultiplexLayer switchTo message");
                return;
            }
            this.removeChild(m_pLayers[(int)m_nEnabledLayer], true);

            //[layers replaceObjectAtIndex:enabledLayer withObject:[NSNull null]];
            m_pLayers[(int)m_nEnabledLayer] = null;

            m_nEnabledLayer = n;

            this.addChild(m_pLayers[(int)n]);
        }
    }
}
