using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ShevaEngine.UI
{    
    /// <summary>
    /// Layer selection.
    /// </summary>
    public class LayerSelection 
    {
        private readonly Layer _layer;
        private Vector2 _actualPoint = Vector2.Zero;        
        private List<Control> _selectableControls;


        /// <summary>
        /// Constructor.
        /// </summary>        
        public LayerSelection(Layer layer)
        {
            _layer = layer;
            _selectableControls = new List<Control>();
        }        
        
        /// <summary>
        /// Find next 
        /// </summary>        
        public bool FindNextSelectedControl(Vector2 direction)
        {
            _layer.GetSelectableControls(_selectableControls);

            if (_selectableControls.Count == 0)
                return false;
            


            return true;
        }
    }
}
