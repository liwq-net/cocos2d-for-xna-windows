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
    /// @brief Base class for CCGrid3D actions.
    /// Grid3D actions can modify a non-tiled grid.
    /// </summary>
    public class CCGrid3DAction : CCGridAction
    {
        /// <summary>
        /// returns the grid
        /// </summary>
        public override CCGridBase getGrid()
        {
            return CCGrid3D.gridWithSize(m_sGridSize);
        }

        /// <summary>
        ///  returns the vertex than belongs to certain position in the grid
        /// </summary>
        public ccVertex3F vertex(ccGridSize pos)
        {
            CCGrid3D g = (CCGrid3D)(m_pTarget.Grid);
            return g.vertex(pos);
        }

        /// <summary>
        ///  returns the non-transformed vertex than belongs to certain position in the grid
        /// </summary>
        public ccVertex3F originalVertex(ccGridSize pos)
        {
            CCGrid3D g = (CCGrid3D)m_pTarget.Grid;
            return g.originalVertex(pos);
        }

        public ccVertex3F originalVertex(int i, int j)
        {
            CCGrid3D g = (CCGrid3D)m_pTarget.Grid;
            return g.originalVertex(i, j);
        }

        /// <summary>
        /// sets a new vertex to a certain position of the grid
        /// </summary>
        public void setVertex(ccGridSize pos, ccVertex3F vertex)
        {
            CCGrid3D g = (CCGrid3D)m_pTarget.Grid;
            g.setVertex(pos, vertex);
        }

        public void setVertex(int i, int j, ccVertex3F vertex)
        {
            CCGrid3D g = (CCGrid3D)m_pTarget.Grid;
            g.setVertex(i, j, vertex);
        }

        /// <summary>
        /// creates the action with size and duration 
        /// </summary>
        public new static CCGrid3DAction actionWithSize(ccGridSize gridSize, float duration)
        {
            throw new NotImplementedException("win32 is not implemented");
        }
    }
}
