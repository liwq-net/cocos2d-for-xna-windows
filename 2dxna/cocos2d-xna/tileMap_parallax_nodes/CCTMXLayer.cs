/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2008-2009 Jason Booth
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
using System.Diagnostics;

namespace cocos2d
{
    public class CCTMXLayer : CCSpriteBatchNode
    {
        #region properties

        protected CCSize m_tLayerSize;
        /// <summary>
        /// size of the layer in tiles
        /// </summary>
        public CCSize LayerSize
        {
            get { return m_tLayerSize; }
            set { m_tLayerSize = value; }
        }

        protected CCSize m_tMapTileSize;
        /// <summary>
        /// size of the map's tile (could be differnt from the tile's size)
        /// </summary>
        public CCSize MapTileSize
        {
            get { return m_tMapTileSize; }
            set { m_tMapTileSize = value; }
        }

        protected int[] m_pTiles;
        /// <summary>
        /// pointer to the map of tiles
        /// </summary>
        public int[] Tiles
        {
            get { return m_pTiles; }
            set { m_pTiles = value; }
        }

        protected CCTMXTilesetInfo m_pTileSet;
        /// <summary>
        /// Tilset information for the layer
        /// </summary>
        public virtual CCTMXTilesetInfo TileSet
        {
            get { return m_pTileSet; }
            set { m_pTileSet = value; }
        }

        protected CCTMXOrientatio m_uLayerOrientation;
        /// <summary>
        /// Layer orientation, which is the same as the map orientation
        /// </summary>
        public CCTMXOrientatio LayerOrientation
        {
            get { return m_uLayerOrientation; }
            set { m_uLayerOrientation = value; }
        }

        protected Dictionary<string, string> m_pProperties;
        /// <summary>
        /// properties from the layer. They can be added using Tiled
        /// </summary>
        public Dictionary<string, string> Properties
        {
            get { return m_pProperties; }
            set { m_pProperties = value; }
        }

        protected string m_sLayerName;
        /// <summary>
        /// ! name of the layer
        /// </summary>
        public string LayerName
        {
            get { return m_sLayerName; }
            set { m_sLayerName = value; }
        }
        //! TMX Layer supports opacity
        protected byte m_cOpacity;

        protected int m_uMinGID;
        protected int m_uMaxGID;

        //! Only used when vertexZ is used
        protected int m_nVertexZvalue;
        protected bool m_bUseAutomaticVertexZ;
        protected float m_fAlphaFuncValue;

        //! used for optimization
        protected CCSprite m_pReusedTile;
        protected ccCArray m_pAtlasIndexArray;

        // used for retina display
        protected float m_fContentScaleFactor;

        #endregion

        public CCTMXLayer()
        {

        }

        /// <summary>
        /// creates a CCTMXLayer with an tileset info, a layer info and a map info
        /// </summary>
        public static CCTMXLayer layerWithTilesetInfo(CCTMXTilesetInfo tilesetInfo, CCTMXLayerInfo layerInfo, CCTMXMapInfo mapInfo)
        {
            CCTMXLayer pRet = new CCTMXLayer();
            if (pRet.initWithTilesetInfo(tilesetInfo, layerInfo, mapInfo))
            {
                return pRet;
            }
            return null;
        }

        /// <summary>
        /// initializes a CCTMXLayer with a tileset info, a layer info and a map info 
        /// </summary>
        public bool initWithTilesetInfo(CCTMXTilesetInfo tilesetInfo, CCTMXLayerInfo layerInfo, CCTMXMapInfo mapInfo)
        {
            // XXX: is 35% a good estimate ?
            CCSize size = layerInfo.m_tLayerSize;
            float totalNumberOfTiles = size.width * size.height;
            float capacity = totalNumberOfTiles * 0.35f + 1; // 35 percent is occupied ?

            CCTexture2D texture = null;
            if (tilesetInfo != null)
            {
                texture = CCTextureCache.sharedTextureCache().addImage(tilesetInfo.m_sSourceImage);
            }

            if (base.initWithTexture(texture, (int)capacity))
            {
                // layerInfo
                m_sLayerName = layerInfo.m_sName;
                m_tLayerSize = layerInfo.m_tLayerSize;
                m_pTiles = layerInfo.m_pTiles;
                m_uMinGID = layerInfo.m_uMinGID;
                m_uMaxGID = layerInfo.m_uMaxGID;
                m_cOpacity = layerInfo.m_cOpacity;
                m_pProperties = layerInfo.Properties;
                //			m_pProperties = CCStringToStringDictionary::dictionaryWithDictionary(layerInfo->getProperties());
                m_fContentScaleFactor = CCDirector.sharedDirector().ContentScaleFactor;

                // tilesetInfo
                m_pTileSet = tilesetInfo;
                //CC_SAFE_RETAIN(m_pTileSet);

                // mapInfo
                m_tMapTileSize = mapInfo.TileSize;
                m_uLayerOrientation = (CCTMXOrientatio)mapInfo.Orientation;

                // offset (after layer orientation is set);
                CCPoint offset = this.calculateLayerOffset(layerInfo.m_tOffset);
                this.position = offset;

                m_pAtlasIndexArray = ccCArray.ccCArrayNew((int)totalNumberOfTiles);

                this.contentSizeInPixels = new CCSize(m_tLayerSize.width * m_tMapTileSize.width, m_tLayerSize.height * m_tMapTileSize.height);
                m_tMapTileSize.width /= m_fContentScaleFactor;
                m_tMapTileSize.height /= m_fContentScaleFactor;

                m_bUseAutomaticVertexZ = false;
                m_nVertexZvalue = 0;
                m_fAlphaFuncValue = 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// dealloc the map that contains the tile position from memory.
        /// Unless you want to know at runtime the tiles positions, you can safely call this method.
        /// If you are going to call layer->tileGIDAt() then, don't release the map
        /// </summary>
        public void releaseMap()
        {
            if (m_pTiles != null)
            {
                m_pTiles = null;
            }

            if (m_pAtlasIndexArray != null)
            {
                m_pAtlasIndexArray = null;
            }
        }

        /// <summary>
        ///  returns the tile (CCSprite) at a given a tile coordinate.
        ///  The returned CCSprite will be already added to the CCTMXLayer. Don't add it again.
        ///  The CCSprite can be treated like any other CCSprite: rotated, scaled, translated, opacity, color, etc.
        ///  You can remove either by calling:
        /// - layer->removeChild(sprite, cleanup);
        /// - or layer->removeTileAt(ccp(x,y));
        /// </summary>
        public CCSprite tileAt(CCPoint pos)
        {
            Debug.Assert(pos.x < m_tLayerSize.width && pos.y < m_tLayerSize.height && pos.x >= 0 && pos.y >= 0, "TMXLayer: invalid position");

            Debug.Assert(m_pTiles != null && m_pAtlasIndexArray != null, "TMXLayer: the tiles map has been released");

            CCSprite tile = null;
            int gid = this.tileGIDAt(pos);

            // if GID == 0, then no tile is present
            if (gid != 0)
            {
                int z = (int)(pos.x + pos.y * m_tLayerSize.width);
                tile = (CCSprite)this.getChildByTag(z);

                // tile not created yet. create it
                if (tile == null)
                {
                    CCRect rect = m_pTileSet.rectForGID(gid);
                    rect = new CCRect(rect.origin.x / m_fContentScaleFactor, rect.origin.y / m_fContentScaleFactor, rect.size.width / m_fContentScaleFactor, rect.size.height / m_fContentScaleFactor);

                    tile = new CCSprite();
                    tile.initWithBatchNode(this, rect);
                    tile.position = positionAt(pos);
                    tile.vertexZ = (float)vertexZForPos(pos);
                    tile.anchorPoint = new CCPoint(0, 0);
                    tile.Opacity = m_cOpacity;

                    int indexForZ = atlasIndexForExistantZ(z);
                    this.addSpriteWithoutQuad(tile, indexForZ, z);
                }
            }
            return tile;
        }

        /// <summary>
        /// returns the tile gid at a given tile coordinate.
        ///	if it returns 0, it means that the tile is empty.
        ///	This method requires the the tile map has not been previously released (eg. don't call layer->releaseMap())
        /// </summary>
        public int tileGIDAt(CCPoint pos)
        {
            Debug.Assert(pos.x < m_tLayerSize.width && pos.y < m_tLayerSize.height && pos.x >= 0 && pos.y >= 0, "TMXLayer: invalid position");
            Debug.Assert(m_pTiles != null && m_pAtlasIndexArray != null, "TMXLayer: the tiles map has been released");

            int idx = (int)(pos.x + pos.y * m_tLayerSize.width);
            return m_pTiles[idx];
        }

        /// <summary>
        /// sets the tile gid (gid = tile global id) at a given tile coordinate.
        /// The Tile GID can be obtained by using the method "tileGIDAt" or by using the TMX editor -> Tileset Mgr +1.
        /// If a tile is already placed at that position, then it will be removed.
        /// </summary>
        public void setTileGID(int gid, CCPoint pos)
        {
            Debug.Assert(pos.x < m_tLayerSize.width && pos.y < m_tLayerSize.height && pos.x >= 0 && pos.y >= 0, "TMXLayer: invalid position");
            Debug.Assert(m_pTiles != null && m_pAtlasIndexArray != null, "TMXLayer: the tiles map has been released");
            Debug.Assert(gid == 0 || gid >= m_pTileSet.m_uFirstGid, "TMXLayer: invalid gid");

            int currentGID = tileGIDAt(pos);

            if (currentGID != gid)
            {
                // setting gid=0 is equal to remove the tile
                if (gid == 0)
                {
                    removeTileAt(pos);
                }

                // empty tile. create a new one
                else if (currentGID == 0)
                {
                    insertTileForGID(gid, pos);
                }

                // modifying an existing tile with a non-empty tile
                else
                {
                    int z = (int)(pos.x + pos.y * m_tLayerSize.width);
                    CCSprite sprite = (CCSprite)getChildByTag((int)z);
                    if (sprite != null)
                    {
                        CCRect rect = m_pTileSet.rectForGID(gid);
                        rect = new CCRect(rect.origin.x / m_fContentScaleFactor, rect.origin.y / m_fContentScaleFactor, rect.size.width / m_fContentScaleFactor, rect.size.height / m_fContentScaleFactor);

                        sprite.setTextureRectInPixels(rect, false, rect.size);
                        m_pTiles[z] = gid;
                    }
                    else
                    {
                        updateTileForGID(gid, pos);
                    }
                }
            }
        }

        /// <summary>
        /// removes a tile at given tile coordinate
        /// </summary>
        public void removeTileAt(CCPoint pos)
        {
            Debug.Assert(pos.x < m_tLayerSize.width && pos.y < m_tLayerSize.height && pos.x >= 0 && pos.y >= 0, "TMXLayer: invalid position");
            Debug.Assert(m_pTiles != null && m_pAtlasIndexArray != null, "TMXLayer: the tiles map has been released");

            int gid = tileGIDAt(pos);

            if (gid != 0)
            {
                int z = (int)(pos.x + pos.y * m_tLayerSize.width);
                int atlasIndex = atlasIndexForExistantZ(z);

                // remove tile from GID map
                m_pTiles[z] = 0;

                // remove tile from atlas position array
                ccCArray.ccCArrayRemoveValueAtIndex(m_pAtlasIndexArray, atlasIndex);

                // remove it from sprites and/or texture atlas
                CCSprite sprite = (CCSprite)getChildByTag((int)z);
                if (sprite != null)
                {
                    base.removeChild(sprite, true);
                }
                else
                {
                    m_pobTextureAtlas.removeQuadAtIndex(atlasIndex);

                    // update possible children
                    if (m_pChildren != null && m_pChildren.Count > 0)
                    {
                        CCObject pObject = null;
                        for (int i = 0; i < m_pChildren.Count; i++)
                        {
                            CCSprite pChild = (CCSprite)pObject;
                            if (pChild != null)
                            {
                                int ai = pChild.atlasIndex;
                                if (ai >= atlasIndex)
                                {
                                    pChild.atlasIndex = ai - 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// returns the position in pixels of a given tile coordinate
        /// </summary>
        public CCPoint positionAt(CCPoint pos)
        {
            CCPoint ret = new CCPoint(0, 0);
            switch (m_uLayerOrientation)
            {
                case CCTMXOrientatio.CCTMXOrientationOrtho:
                    ret = positionForOrthoAt(pos);
                    break;
                case CCTMXOrientatio.CCTMXOrientationIso:
                    ret = positionForIsoAt(pos);
                    break;
                case CCTMXOrientatio.CCTMXOrientationHex:
                    ret = positionForHexAt(pos);
                    break;
            }
            return ret;
        }

        /// <summary>
        /// return the value for the specific property name
        /// </summary>
        public string propertyNamed(string propertyName)
        {
            return m_pProperties[propertyName];
        }

        /// <summary>
        ///  Creates the tiles
        /// </summary>
        public void setupTiles()
        {
            // Optimization: quick hack that sets the image size on the tileset
            m_pTileSet.m_tImageSize = m_pobTextureAtlas.Texture.ContentSizeInPixels;

            // By default all the tiles are aliased
            // pros:
            //  - easier to render
            // cons:
            //  - difficult to scale / rotate / etc.
            //m_pobTextureAtlas.Texture.setAliasTexParameters();

            //CFByteOrder o = CFByteOrderGetCurrent();

            // Parse cocos2d properties
            //this.parseInternalProperties();

            for (int y = 0; y < m_tLayerSize.height; y++)
            {
                for (int x = 0; x < m_tLayerSize.width; x++)
                {
                    int pos = (int)(x + m_tLayerSize.width * y);
                    int gid = m_pTiles[pos];

                    // gid are stored in little endian.
                    // if host is big endian, then swap
                    //if( o == CFByteOrderBigEndian )
                    //	gid = CFSwapInt32( gid );
                    /* We support little endian.*/

                    // XXX: gid == 0 --> empty tile
                    if (gid != 0)
                    {
                        var reusedTile = this.appendTileForGID(gid, new CCPoint((float)x, (float)y));

                        // Optimization: update min and max GID rendered by the layer
                        m_uMinGID = Math.Min(gid, m_uMinGID);
                        m_uMaxGID = Math.Max(gid, m_uMaxGID);
                    }
                }
            }
            Debug.Assert(m_uMaxGID >= m_pTileSet.m_uFirstGid && m_uMinGID >= m_pTileSet.m_uFirstGid, "TMX: Only 1 tilset per layer is supported");
        }

        /// <summary>
        /// CCTMXLayer doesn't support adding a CCSprite manually.
        /// @warning addchild(z, tag); is not supported on CCTMXLayer. Instead of setTileGID.
        /// </summary>
        public override void addChild(CCNode child, int zOrder, int tag)
        {
            // Debug.Assert(false, "addChild: is not supported on CCTMXLayer. Instead use setTileGID:at:/tileAt:");
            base.addChild(child, zOrder, tag);
        }

        // super method
        public override void removeChild(CCNode child, bool cleanup)
        {
            CCSprite sprite = child as CCSprite;
            // allows removing nil objects
            if (sprite == null)
                return;

            Debug.Assert(m_pChildren.Contains(sprite), "Tile does not belong to TMXLayer");

            int atlasIndex = sprite.atlasIndex;
            int zz = m_pAtlasIndexArray.arr[atlasIndex];
            m_pTiles[zz] = 0;
            ccCArray.ccCArrayRemoveValueAtIndex(m_pAtlasIndexArray, atlasIndex);

        }

        private CCPoint positionForIsoAt(CCPoint pos)
        {
            CCPoint xy = new CCPoint(m_tMapTileSize.width / 2 * (m_tLayerSize.width + pos.x - pos.y - 1),
                                 m_tMapTileSize.height / 2 * ((m_tLayerSize.height * 2 - pos.x - pos.y) - 2));
            return xy;
        }
        private CCPoint positionForOrthoAt(CCPoint pos)
        {
            CCPoint xy = new CCPoint(pos.x * m_tMapTileSize.width,
                                (m_tLayerSize.height - pos.y - 1) * m_tMapTileSize.height);
            return xy;
        }
        private CCPoint positionForHexAt(CCPoint pos)
        {
            float diffY = 0;
            if ((int)pos.x % 2 == 1)
            {
                diffY = -m_tMapTileSize.height / 2;
            }

            CCPoint xy = new CCPoint(pos.x * m_tMapTileSize.width * 3 / 4,
                                    (m_tLayerSize.height - pos.y - 1) * m_tMapTileSize.height + diffY);
            return xy;
        }

        private CCPoint calculateLayerOffset(CCPoint pos)
        {
            CCPoint ret = new CCPoint(0, 0);
            switch (m_uLayerOrientation)
            {
                case CCTMXOrientatio.CCTMXOrientationOrtho:
                    ret = new CCPoint(pos.x * m_tMapTileSize.width, -pos.y * m_tMapTileSize.height);
                    break;
                case CCTMXOrientatio.CCTMXOrientationHex:
                    Debug.Assert(CCPoint.CCPointEqualToPoint(pos, new CCPoint(0, 0)), "offset for hexagonal map not implemented yet");
                    break;
                case CCTMXOrientatio.CCTMXOrientationIso:
                    ret = new CCPoint((m_tMapTileSize.width / 2) * (pos.x - pos.y),
                                   (m_tMapTileSize.height / 2) * (-pos.x - pos.y));
                    break;
            }
            return ret;
        }

        /// <summary>
        /// optimization methos
        /// </summary>
        private CCSprite appendTileForGID(int gid, CCPoint pos)
        {
            CCRect rect = m_pTileSet.rectForGID(gid);
            rect = new CCRect(rect.origin.x / m_fContentScaleFactor, rect.origin.y / m_fContentScaleFactor, rect.size.width / m_fContentScaleFactor, rect.size.height / m_fContentScaleFactor);

            int z = (int)(pos.x + pos.y * m_tLayerSize.width);

            if (m_pReusedTile == null)
            {
                m_pReusedTile = new CCSprite();
                m_pReusedTile.initWithBatchNode(this, rect);
            }
            else
            {
                m_pReusedTile = new CCSprite();
                m_pReusedTile.initWithBatchNode(this, rect);
            }

            m_pReusedTile.position = positionAt(pos);
            m_pReusedTile.vertexZ = (float)vertexZForPos(pos);
            m_pReusedTile.anchorPoint = new CCPoint(0, 0);
            m_pReusedTile.Opacity = 255;

            // optimization:
            // The difference between appendTileForGID and insertTileforGID is that append is faster, since
            // it appends the tile at the end of the texture atlas
            int indexForZ = m_pAtlasIndexArray.num;

            // don't add it using the "standard" way.
            addQuadFromSprite(m_pReusedTile, indexForZ);

            // append should be after addQuadFromSprite since it modifies the quantity values
            ccCArray.ccCArrayInsertValueAtIndex(m_pAtlasIndexArray, z, indexForZ);

            return m_pReusedTile;
        }

        private CCSprite insertTileForGID(int gid, CCPoint pos)
        {
            CCRect rect = m_pTileSet.rectForGID(gid);
            rect = new CCRect(rect.origin.x / m_fContentScaleFactor, rect.origin.y / m_fContentScaleFactor, rect.size.width / m_fContentScaleFactor, rect.size.height / m_fContentScaleFactor);

            int z = (int)(pos.x + pos.y * m_tLayerSize.width);

            if (m_pReusedTile == null)
            {
                m_pReusedTile = new CCSprite();
                m_pReusedTile.initWithBatchNode(this, rect);
            }
            else
            {
                m_pReusedTile = new CCSprite();
                m_pReusedTile.initWithBatchNode(this, rect);
            }
            m_pReusedTile.positionInPixels = positionAt(pos);
            m_pReusedTile.vertexZ = (float)vertexZForPos(pos);
            m_pReusedTile.anchorPoint = new CCPoint(0, 0);
            m_pReusedTile.Opacity = m_cOpacity;

            // get atlas index
            int indexForZ = atlasIndexForNewZ(z);

            // Optimization: add the quad without adding a child
            this.addQuadFromSprite(m_pReusedTile, indexForZ);

            // insert it into the local atlasindex array
            ccCArray.ccCArrayInsertValueAtIndex(m_pAtlasIndexArray, z, indexForZ);

            // update possible children
            if (m_pChildren != null && m_pChildren.Count > 0)
            {
                CCObject pObject = null;

                for (int i = 0; i < m_pChildren.Count; i++)
                {
                    CCSprite pChild = (CCSprite)pObject;
                    if (pChild != null)
                    {
                        int ai = pChild.atlasIndex;
                        if (ai >= indexForZ)
                        {
                            pChild.atlasIndex = ai + 1;
                        }
                    }
                }
            }
            m_pTiles[z] = gid;
            return m_pReusedTile;
        }
        private CCSprite updateTileForGID(int gid, CCPoint pos)
        {
            CCRect rect = m_pTileSet.rectForGID(gid);
            rect = new CCRect(rect.origin.x / m_fContentScaleFactor, rect.origin.y / m_fContentScaleFactor, rect.size.width / m_fContentScaleFactor, rect.size.height / m_fContentScaleFactor);
            int z = (int)(pos.x + pos.y * m_tLayerSize.width);

            if (m_pReusedTile == null)
            {
                m_pReusedTile = new CCSprite();
                m_pReusedTile.initWithBatchNode(this, rect);
            }
            else
            {
                m_pReusedTile = new CCSprite();
                m_pReusedTile.initWithBatchNode(this, rect);
            }

            m_pReusedTile.positionInPixels = positionAt(pos);
            m_pReusedTile.vertexZ = (float)vertexZForPos(pos);
            m_pReusedTile.anchorPoint = new CCPoint(0, 0);
            m_pReusedTile.Opacity = m_cOpacity;

            // get atlas index
            int indexForZ = atlasIndexForExistantZ(z);
            m_pReusedTile.atlasIndex = indexForZ;
            m_pReusedTile.dirty = true;
            m_pReusedTile.updateTransform();
            m_pTiles[z] = gid;

            return m_pReusedTile;
        }

        /// <summary>
        /// The layer recognizes some special properties, like cc_vertez 
        /// </summary>
        private void parseInternalProperties()
        {
            string vertexz = propertyNamed("cc_vertexz");
            if (vertexz != null)
            {
                if (vertexz == "automatic")
                {
                    m_bUseAutomaticVertexZ = true;
                }
                else
                {
                    m_nVertexZvalue = ccUtils.ccParseInt(vertexz);
                }
            }

            string alphaFuncVal = propertyNamed("cc_alpha_func");
            if (alphaFuncVal != null)
            {
                m_fAlphaFuncValue = ccUtils.ccParseFloat(alphaFuncVal);
            }
        }
        private int vertexZForPos(CCPoint pos)
        {
            int ret = 0;
            uint maxVal = 0;
            if (m_bUseAutomaticVertexZ)
            {
                switch (m_uLayerOrientation)
                {
                    case CCTMXOrientatio.CCTMXOrientationOrtho:
                        ret = (int)(-(m_tLayerSize.height - pos.y));
                        break;
                    case CCTMXOrientatio.CCTMXOrientationHex:
                        Debug.Assert(false, "TMX Hexa zOrder not supported");
                        break;
                    case CCTMXOrientatio.CCTMXOrientationIso:

                        maxVal = (uint)(m_tLayerSize.width + m_tLayerSize.height);
                        ret = (int)(-(maxVal - (pos.x + pos.y)));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                ret = m_nVertexZvalue;
            }
            return ret;
        }

        int compareInts(object a, object b)
        {
            return ((int)a - (int)b);
        }
        // index
        private int atlasIndexForExistantZ(int z)
        {
            int key = z;
            //int item = (int)bsearch((void*)&key, (void*)&m_pAtlasIndexArray.arr[0], m_pAtlasIndexArray.num, sizeof(object), compareInts);

            //Debug.Assert(item>0, "TMX atlas index not found. Shall not happen");

            //int index = ((int)item - (int)m_pAtlasIndexArray->arr) / sizeof(void*);
            //return index;
            int result = Array.IndexOf<int>(m_pAtlasIndexArray.arr, z);

            return result;
        }
        private int atlasIndexForNewZ(int z)
        {
            // XXX: This can be improved with a sort of binary search
            int i = 0;
            for (i = 0; i < m_pAtlasIndexArray.num; i++)
            {
                int val = (int)m_pAtlasIndexArray.arr[i];
                if (z < val)
                    break;
            }
            return i;
        }
    }
}
