/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2008-2011 Ricardo Quesada
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
using System.Diagnostics;

namespace cocos2d
{
    public enum tCCMenuState
    {
        kCCMenuStateWaiting,
        kCCMenuStateTrackingTouch
    };

    /// <summary>
    /// A CCMenu
    /// Features and Limitation:
    ///  You can add MenuItem objects in runtime using addChild:
    ///  But the only accecpted children are MenuItem objects
    /// </summary>
    public class CCMenu : CCLayer, ICCRGBAProtocol, ICCTouchDelegate
    {
        public const float kDefaultPadding = 5;
        public const int kCCMenuTouchPriority = -128;

        protected tCCMenuState m_eState;
        protected CCMenuItem m_pSelectedItem;

        public CCMenu()
        {
            m_cOpacity = 0;
            m_pSelectedItem = null;
        }

        /// <summary>
        /// creates an empty CCMenu
        /// </summary>
        public static CCMenu node()
        {
            CCMenu menu = new CCMenu();

            if (menu != null && menu.init())
            {
                return menu;
            }

            return null;
        }

        /// <summary>
        /// creates a CCMenu with it's items
        /// </summary>
        public static CCMenu menuWithItems(params CCMenuItem[] item)
        {
            CCMenu pRet = new CCMenu();

            if (pRet != null && pRet.initWithItems(item))
            {
                return pRet;
            }

            return null;
        }

        /// <summary>
        /// creates a CCMenu with it's item, then use addChild() to add 
        /// other items. It is used for script, it can't init with undetermined
        /// number of variables.
        /// </summary>
        public static CCMenu menuWithItem(CCMenuItem item)
        {
            return menuWithItems(item);
        }

        /// <summary>
        /// initializes an empty CCMenu
        /// </summary>
        public bool init()
        {
            return initWithItems(null);
        }

        /// <summary>
        /// initializes a CCMenu with it's items 
        /// </summary>
        bool initWithItems(params CCMenuItem[] item)
        {
            if (base.init())
            {
                this.m_bIsTouchEnabled = true;

                // menu in the center of the screen
                CCSize s = CCDirector.sharedDirector().getWinSize();

                this.m_bIsRelativeAnchorPoint = false;
                anchorPoint = new CCPoint(0.5f, 0.5f);
                this.contentSize = s;

                // XXX: in v0.7, winSize should return the visible size
                // XXX: so the bar calculation should be done there
                CCRect r;
                CCApplication.sharedApplication().statusBarFrame(out r);

                ccDeviceOrientation orientation = CCDirector.sharedDirector().deviceOrientation;
                if (orientation == ccDeviceOrientation.CCDeviceOrientationLandscapeLeft
                    ||
                    orientation == ccDeviceOrientation.CCDeviceOrientationLandscapeRight)
                {
                    s.height -= r.size.width;
                }
                else
                {
                    s.height -= r.size.height;
                }

                position = new CCPoint(s.width / 2, s.height / 2);

                if (item != null)
                {
                    foreach (var menuItem in item)
                    {
                        this.addChild(menuItem);
                    }
                }
                //	[self alignItemsVertically];

                m_pSelectedItem = null;
                m_eState = tCCMenuState.kCCMenuStateWaiting;
                return true;
            }

            return false;
        }

        /// <summary>
        /// align items vertically
        /// </summary>
        public void alignItemsVertically()
        {
            this.alignItemsVerticallyWithPadding(kDefaultPadding);
        }

        /// <summary>
        /// align items vertically with padding
        /// @since v0.7.2
        /// </summary>
        public void alignItemsVerticallyWithPadding(float padding)
        {
            float height = -padding;

            if (m_pChildren != null && m_pChildren.Count > 0)
            {
                foreach (var pChild in m_pChildren)
                {
                    if (pChild != null)
                    {
                        height += pChild.contentSize.height * pChild.scaleY + padding;
                    }
                }
            }

            float y = height / 2.0f;
            if (m_pChildren != null && m_pChildren.Count > 0)
            {
                foreach (var pChild in m_pChildren)
                {
                    if (pChild != null)
                    {
                        pChild.position = new CCPoint(0, y - pChild.contentSize.height * pChild.scaleY / 2.0f);
                        y -= pChild.contentSize.height * pChild.scaleY + padding;
                    }
                }
            }
        }

        /// <summary>
        /// align items horizontally
        /// </summary>
        public void alignItemsHorizontally()
        {
            this.alignItemsHorizontallyWithPadding(kDefaultPadding);
        }

        /// <summary>
        /// align items horizontally with padding
        /// @since v0.7.2
        /// </summary>
        public void alignItemsHorizontallyWithPadding(float padding)
        {
            float width = -padding;

            if (m_pChildren != null && m_pChildren.Count > 0)
            {
                foreach (var pChild in m_pChildren)
                {
                    if (pChild != null)
                    {
                        width += pChild.contentSize.width * pChild.scaleX + padding;
                    }
                }
            }

            float x = -width / 2.0f;
            if (m_pChildren != null && m_pChildren.Count > 0)
            {
                foreach (var pChild in m_pChildren)
                {
                    if (pChild != null)
                    {
                        pChild.position = new CCPoint(x + pChild.contentSize.width * pChild.scaleX / 2.0f, 0);
                        x += pChild.contentSize.width * pChild.scaleX + padding;
                    }
                }
            }
        }

        /** align items in rows of columns */
        public void alignItemsInColumns(params int[] columns)
        {
            int[] rows = columns;

            int height = -5;
            int row = 0;
            int rowHeight = 0;
            int columnsOccupied = 0;
            int rowColumns;

            if (m_pChildren != null  && m_pChildren.Count > 0)
            {
                foreach (CCNode pChild in m_pChildren)
                {
                    if (null != pChild)
                    {
                        Debug.Assert(row < rows.Length);

                        rowColumns = rows[row];
                        // can not have zero columns on a row
                        Debug.Assert(rowColumns > 0);

                        float tmp = pChild.contentSize.height;
                        rowHeight = (int)((rowHeight >= tmp) ? rowHeight : tmp);

                        ++columnsOccupied;
                        if (columnsOccupied >= rowColumns)
                        {
                            height += rowHeight + (int)kDefaultPadding;

                            columnsOccupied = 0;
                            rowHeight = 0;
                            ++row;
                        }
                    }
                }
            }	

            // check if too many rows/columns for available menu items
            //assert(! columnsOccupied);

            CCSize winSize = CCDirector.sharedDirector().getWinSize();

            row = 0;
            rowHeight = 0;
            rowColumns = 0;
            float w = 0.0f;
            float x = 0.0f;
            float y = (float)(height / 2);

            if (m_pChildren != null && m_pChildren.Count > 0)
            {
                foreach (CCNode pChild in m_pChildren)
                {
                    if (pChild != null)
                    {
                        if (rowColumns == 0)
                        {
                            rowColumns = rows[row];
                            if (rowColumns == 0)
                            {
                                throw (new ArgumentException("Can not have a zero column size for a row."));
                            }
                            w = (winSize.width - 2 * kDefaultPadding) / rowColumns; // 1 + rowColumns
                            x = w/2f; // center of column
                        }

                        float tmp = pChild.contentSize.height*pChild.scaleY;
                        rowHeight = (int)((rowHeight >= tmp) ? rowHeight : tmp);

                        pChild.position = new CCPoint(kDefaultPadding + x - (winSize.width - 2*kDefaultPadding) / 2,
                                               y - pChild.contentSize.height*pChild.scaleY / 2);

                        x += w;
                        ++columnsOccupied;

                        if (columnsOccupied >= rowColumns)
                        {
                            y -= rowHeight + 5;

                            columnsOccupied = 0;
                            rowColumns = 0;
                            rowHeight = 0;
                            ++row;
                        }
                    }
                }
            }	
        }

        /** align items in columns of rows */
        public void alignItemsInRows(params int[] rows)
        {
            int[] columns = rows;

            List<int> columnWidths = new List<int>();
		    List<int> columnHeights = new List<int>();

		    int width = -10;
		    int columnHeight = -5;
		    int column = 0;
		    int columnWidth = 0;
		    int rowsOccupied = 0;
		    int columnRows;

		    if (null != m_pChildren && m_pChildren.Count > 0)
		    {
                foreach (CCNode pChild in m_pChildren)
                {
                    if (null != pChild)
                    {
                        // check if too many menu items for the amount of rows/columns
				        Debug.Assert(column < columns.Length);

				        columnRows = columns[column];
				        // can't have zero rows on a column
				        Debug.Assert(columnRows > 0);

				        // columnWidth = fmaxf(columnWidth, [item contentSize].width);
				        float tmp = pChild.contentSize.width * pChild.scaleX;
				        columnWidth = (int)((columnWidth >= tmp) ? columnWidth : tmp);

				        columnHeight += (int)(pChild.contentSize.height*pChild.scaleY + 5);
				        ++rowsOccupied;

				        if (rowsOccupied >= columnRows)
				        {
					        columnWidths.Add(columnWidth);
					        columnHeights.Add(columnHeight);
					        width += columnWidth + 10;

					        rowsOccupied = 0;
					        columnWidth = 0;
					        columnHeight = -5;
					        ++column;
				        }
                    }
                }
		    }

		    // check if too many rows/columns for available menu items.
		    Debug.Assert(0 == rowsOccupied);

		    CCSize winSize = CCDirector.sharedDirector().getWinSize();

		    column = 0;
		    columnWidth = 0;
		    columnRows = 0;
		    float x = (float)(-width / 2);
		    float y = 0.0f;

            if (null != m_pChildren && m_pChildren.Count > 0)
		    {
                foreach (CCNode pChild in m_pChildren)
                {
                    if (null != pChild)
                    {
                        if (columnRows == 0)
				        {
					        columnRows = columns[column];
					        y = (float) columnHeights[column];
				        }

				        // columnWidth = fmaxf(columnWidth, [item contentSize].width);
				        float tmp = pChild.contentSize.width*pChild.scaleX;
				        columnWidth = (int)((columnWidth >= tmp) ? columnWidth : tmp);

				        pChild.position = new CCPoint(x + columnWidths[column] / 2,
					                                  y - winSize.height / 2);

				        y -= pChild.contentSize.height*pChild.scaleY + 10;
				        ++rowsOccupied;

				        if (rowsOccupied >= columnRows)
				        {
					        x += columnWidth + 5;
					        rowsOccupied = 0;
					        columnRows = 0;
					        columnWidth = 0;
					        ++column;
				        }
                    }
                }
		    }
        }

        public override void registerWithTouchDispatcher()
        {
            CCTouchDispatcher.sharedDispatcher().addTargetedDelegate(this, kCCMenuTouchPriority, true);
        }

        /// <summary>
        /// For phone event handle functions
        /// </summary>
        public override bool ccTouchBegan(CCTouch touch, CCEvent ccevent)
        {
            if (m_eState != tCCMenuState.kCCMenuStateWaiting || !m_bIsVisible)
            {
                return false;
            }

            for (CCNode c = this.m_pParent; c != null; c = c.parent)
            {
                if (c.visible == false)
                {
                    return false;
                }
            }

            m_pSelectedItem = this.itemForTouch(touch);

            if (m_pSelectedItem != null)
            {
                m_eState = tCCMenuState.kCCMenuStateTrackingTouch;
                m_pSelectedItem.selected();

                return true;
            }

            return false;
        }

        public override void ccTouchEnded(CCTouch touch, CCEvent ccevent)
        {
            Debug.Assert(m_eState == tCCMenuState.kCCMenuStateTrackingTouch, "[Menu ccTouchEnded] -- invalid state");

            if (m_pSelectedItem != null)
            {
                m_pSelectedItem.unselected();
                m_pSelectedItem.activate();
            }

            m_eState = tCCMenuState.kCCMenuStateWaiting;
        }

        public override void ccTouchCancelled(CCTouch touch, CCEvent ccevent)
        {
            Debug.Assert(m_eState == tCCMenuState.kCCMenuStateTrackingTouch, "[Menu ccTouchCancelled] -- invalid state");

            if (m_pSelectedItem != null)
            {
                m_pSelectedItem.unselected();
            }

            m_eState = tCCMenuState.kCCMenuStateWaiting;
        }

        public override void ccTouchMoved(CCTouch touch, CCEvent ccevent)
        {
            Debug.Assert(m_eState == tCCMenuState.kCCMenuStateTrackingTouch, "[Menu ccTouchMoved] -- invalid state");

            CCMenuItem currentItem = this.itemForTouch(touch);

            if (currentItem != m_pSelectedItem)
            {
                if (m_pSelectedItem != null)
                {
                    m_pSelectedItem.unselected();
                }

                m_pSelectedItem = currentItem;

                if (m_pSelectedItem != null)
                {
                    m_pSelectedItem.selected();
                }
            }
        }

        public virtual void destroy()
        {
            //release();            
        }

        public virtual void keep()
        {
            //throw new NotImplementedException();
        }

        public override void onExit()
        {
            if (m_eState == tCCMenuState.kCCMenuStateTrackingTouch)
            {
                m_pSelectedItem.unselected();
                m_eState = tCCMenuState.kCCMenuStateWaiting;
                m_pSelectedItem = null;
            }

            base.onExit();
        }

        public virtual ICCRGBAProtocol convertToRGBAProtocol()
        {
            return (ICCRGBAProtocol)this;
        }

        protected CCMenuItem itemForTouch(CCTouch touch)
        {
            //XNA point
            CCPoint touchLocation = touch.locationInView(touch.view());
            //cocos2d point
            touchLocation = CCDirector.sharedDirector().convertToGL(touchLocation);

            if (m_pChildren != null && m_pChildren.Count > 0)
            {
                foreach (var pChild in m_pChildren)
                {
                    if (pChild != null && pChild.visible && ((CCMenuItem)pChild).Enabled)
                    {
                        CCPoint local = pChild.convertToNodeSpace(touchLocation);
                        CCRect r = ((CCMenuItem)pChild).rect();
                        r.origin = CCPoint.Zero;

                        if (CCRect.CCRectContainsPoint(r, local))
                        {
                            return (CCMenuItem)pChild;
                        }
                    }
                }
            }

            return null;
        }

        #region CCRGBAProtocol Interface

        protected ccColor3B m_tColor;
        protected byte m_cOpacity;

        public ccColor3B Color
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public byte Opacity
        {
            get
            {
                return m_cOpacity;
            }
            set
            {
                m_cOpacity = value;

                if (m_pChildren != null && m_pChildren.Count > 0)
                {
                    //CCObject pObject = null;
                    //CCARRAY_FOREACH(m_pChildren, pObject)
                    //{
                    //    CCNode pChild = (CCNode) pObject;
                    //    if (pChild != null)
                    //    {
                    //        CCRGBAProtocol pRGBAProtocol = pChild.convertToRGBAProtocol();
                    //        if (pRGBAProtocol)
                    //        {
                    //            pRGBAProtocol->setOpacity(m_cOpacity);
                    //        }
                    //    }
                    //}
                    foreach (CCNode pChild in m_pChildren)
                    {
                        if (pChild != null)
                        {
                            ICCRGBAProtocol pRGBAProtocol = pChild as ICCRGBAProtocol;
                            if (pRGBAProtocol != null)
                            {
                                pRGBAProtocol.Opacity = m_cOpacity;
                            }
                        }
                    }
                }
            }
        }

        public bool IsOpacityModifyRGB
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
