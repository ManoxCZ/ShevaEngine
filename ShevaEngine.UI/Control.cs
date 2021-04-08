using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using ShevaEngine.UI.Brushes;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Control.
    /// </summary> 	
    public abstract class Control : PropertiesClass, IDisposable
    {
		protected readonly Log Log;
		protected ControlFlag Flags { get; set; }
        protected List<IDisposable> Disposables { get; }
        public BehaviorSubject<ModelView> DataContext { get; }		
        public BehaviorSubject<Brush> Background { get; }
        public BehaviorSubject<Brush> Foreground { get; }
        public BehaviorSubject<HorizontalAlignment> HorizontalAlignment { get; }
        public BehaviorSubject<VerticalAlignment> VerticalAlignment { get; }
        public List<Control> Children { get; set; }        
        public bool Enabled { get; set; }
		public bool IsSelectAble
		{
			get => (Flags & ControlFlag.Selectable) == ControlFlag.Selectable;
			set
			{
				if (value)
					Flags |= ControlFlag.Selectable;
				else
					Flags &= ~ControlFlag.Selectable;
			}
		}
		public bool IsSelected
		{
			get => (Flags & ControlFlag.Selected) == ControlFlag.Selected;
			set
			{
				if (value)
					Flags |= ControlFlag.Selected;
				else
					Flags &= ~ControlFlag.Selected;
			}
		}
		public Rectangle LocationSize { get; set; }        
        public string Name { get; set; }                
        public bool Visible { get; set; }
		public BehaviorSubject<Margin> Margin { get; }        
        public BehaviorSubject<int> GridRow { get; set; }
        public BehaviorSubject<int> GridColumn { get; set; }
		public Subject<(InputState InputState, int X, int Y)> Click { get; }
		public Subject<(InputState InputState, int X, int Y)> MouseMove { get; }
		public Subject<(InputState InputState, int Wheel)> MouseWheel { get; }		 
		private int _previousMouseX = 0;
		private int _previousMouseY = 0;
		private int _previousWheel = 0;
		public SortedDictionary<ControlFlag, ControlAnimations> Animations { get; }
        
        private readonly SortedDictionary<string, BehaviorSubject<string>> _bindings;
        private readonly SortedDictionary<string, IDisposable[]> _bindingSources;


        /// <summary>
        /// Constructor.
        /// </summary>
        public Control()
		{
			Log = new Log(GetType());
            
            _bindings = new SortedDictionary<string, BehaviorSubject<string>>();
            _bindingSources = new SortedDictionary<string, IDisposable[]>();

			Flags = ControlFlag.Default;
			Disposables = new List<IDisposable>();
            DataContext = CreateProperty<ModelView>(nameof(DataContext), null);
            Background = CreateProperty<Brush>(nameof(Background), null);
            Foreground = CreateProperty<Brush>(nameof(Foreground), new SolidColorBrush(Color.Black));
            Children = new List<Control>();
			HorizontalAlignment = CreateProperty(nameof(HorizontalAlignment), UI.HorizontalAlignment.Left);
			VerticalAlignment = CreateProperty(nameof(VerticalAlignment), UI.VerticalAlignment.Center);
            GridColumn = CreateProperty(nameof(GridColumn), 0);
            GridRow = CreateProperty(nameof(GridRow), 0);
            Enabled = true;
			IsSelectAble = false;
			IsSelected = false;
			Margin = CreateProperty(nameof(Margin), new Margin());
			Name = GetType().Name;
			Visible = true;			
			Click = new Subject<(InputState InputState, int X, int Y)>();
			MouseMove = new Subject<(InputState InputState, int X, int Y)>();
			MouseWheel = new Subject<(InputState InputState, int Wheel)>();
			Animations = new SortedDictionary<ControlFlag, ControlAnimations>();

			foreach (ControlFlag flag in Enum.GetValues(typeof(ControlFlag)))
				Animations.Add(flag, new ControlAnimations());

            Disposables.Add(DataContext.Subscribe(item =>
            {
                foreach (Control child in Children)
                {
                    child.DataContext.OnNext(item);
                }
            }));            
		}

        

        /// <summary>
        /// Set property value.
        /// </summary>
        public bool SetPropertyBinding(string propertyName, Binding binding)
        {
            string propertyNameLower = propertyName.ToLower();
            string bindingPropertyNameLower = binding.PropertyName.ToLower();

            if (!HasProperty(propertyNameLower))
                return false;

            if (_bindings.ContainsKey(propertyNameLower))
                _bindings[propertyNameLower].OnNext(bindingPropertyNameLower);
            else
            {
                _bindings.Add(propertyNameLower, new BehaviorSubject<string>(bindingPropertyNameLower));
                _bindingSources.Add(propertyNameLower, new IDisposable[3]);

                _bindingSources[propertyNameLower][0] = DataContext.CombineLatest(_bindings[propertyNameLower].DistinctUntilChanged(), (context, bindingProperty) => (context, bindingProperty))
                    .Where(item => item.context != null && item.context.HasProperty(item.bindingProperty))                    
                    .DistinctUntilChanged()
                    .Subscribe(item =>
                    {
                        _bindingSources[propertyNameLower][1]?.Dispose();

                        _bindingSources[propertyNameLower][1] = item.context.Subscribe(item.bindingProperty, newValue =>
                        {
                            SetPropertyValue(propertyNameLower, newValue);
                        });
                        
                        _bindingSources[propertyNameLower][2]?.Dispose();

                        _bindingSources[propertyNameLower][2] = Subscribe(propertyNameLower, newValue =>
                        {
                            item.context.SetPropertyValue(item.bindingProperty, newValue);
                        });                        
                    });
            }

            return true;
        }      

		/// <summary>
		/// Dispose.
		/// </summary>
		public override void Dispose()
		{
            foreach (KeyValuePair<string, IDisposable[]> bindingSourcesItem in _bindingSources)
            {
                bindingSourcesItem.Value[0]?.Dispose();
                bindingSourcesItem.Value[1]?.Dispose();
            }

            _bindingSources.Clear();
            _bindings.Clear();

			foreach (IDisposable item in Disposables)
				item.Dispose();

			Disposables.Clear();
            
			Click.Dispose();
			MouseMove.Dispose();
			MouseWheel.Dispose();

            base.Dispose();
		}

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void InitializeComponent()
        {
            foreach (Control child in Children)
                child.InitializeComponent();
        }		

        /// <summary>
        /// Method resize control.
        /// </summary>
        public virtual void Resize(Rectangle locationSize)
        {
            Margin margin = Margin.Value;

            LocationSize = new Rectangle(
				locationSize.X + margin.Left,
				locationSize.Y + margin.Bottom,
				locationSize.Width - (margin.Left + margin.Right),
				locationSize.Height - (margin.Top + margin.Bottom));

            foreach (Control child in Children)
                child.Resize(LocationSize);
        }       		

        /// <summary>
        /// On click.
        /// </summary>        
        public virtual bool OnMouseClick(InputState inputState)
        {
            if (inputState.MouseState.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                return false;

			if (IsPointInside(inputState.MouseState.X, inputState.MouseState.Y))
			{
				foreach (Control child in Children)
					if (child.Enabled)
					{
						if (child.OnMouseClick(inputState))
							return true;
					}

				if (IsSelectAble && IsPointCollide(inputState.MouseState.X, inputState.MouseState.Y))
				{                    
					Click.OnNext((inputState, inputState.MouseState.X - LocationSize.X, inputState.MouseState.Y - LocationSize.Y));

					return true;
				}
			}
			
            return false;
        }		

		/// <summary>
		/// On click.
		/// </summary>        
		public virtual bool OnMouseMove(InputState inputState)
		{
			bool result = false;

			if (IsPointInside(inputState.MouseState.X, inputState.MouseState.Y))
			{
				foreach (Control child in Children)
					if (child.OnMouseMove(inputState))
						result |= true;

				if (!result)
				{									
					if (IsSelectAble && IsPointCollide(inputState.MouseState.X, inputState.MouseState.Y))
					{
						if (!IsSelected)
						{
							IsSelected = true;

							Animations[ControlFlag.Selected].Start(inputState.Time);
						}

						MouseMove.OnNext((inputState, inputState.MouseState.X - _previousMouseX, inputState.MouseState.Y - _previousMouseY));
						
						result = true;
					}
				}
			}
			else
				SetNotSelected();
			
			_previousMouseX = inputState.MouseState.X;
			_previousMouseY = inputState.MouseState.Y;

			return result;
		}

		/// <summary>
		/// On click.
		/// </summary>        
		public virtual bool OnMouseWheel(InputState inputState)
		{
			if (IsPointInside(inputState.MouseState.X, inputState.MouseState.Y))
			{
				foreach (Control child in Children)
					if (child.Enabled)
					{
						if (child.OnMouseWheel(inputState))
							return true;
					}

				if (IsPointCollide(inputState.MouseState.X, inputState.MouseState.Y))
				{
					MouseWheel.OnNext((inputState, inputState.MouseState.ScrollWheelValue - _previousWheel));

					_previousWheel = inputState.MouseState.ScrollWheelValue;

					return true;
				}
			}

			_previousWheel = inputState.MouseState.ScrollWheelValue;

			return false;
		}

		/// <summary>
		/// Set not selected.
		/// </summary>
		protected virtual void SetNotSelected()
		{
			foreach (Control child in Children)
				child.SetNotSelected();

			IsSelected = false;
		}

		/// <summary>
		/// Is point inside.
		/// </summary>        
		public bool IsPointInside(int x, int y)
        {
            return LocationSize.Contains(new Vector2(x, y));
        }

		/// <summary>
		/// Is point collide.
		/// </summary>
		public virtual bool IsPointCollide(int x, int y)
		{
			return true;
		}

        /// <summary>
        /// Method Draw().
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {            
			foreach (ControlFlag flag in Enum.GetValues(typeof(ControlFlag)))
				if ((Flags & flag) == flag)
					Animations[flag].Update(gameTime);

			
            //if (BackColor.Value != Color.Transparent)
            //    DrawRectangle(spriteBatch, LocationSize, BackColor.Value);

            Background.Value?.Draw(spriteBatch, LocationSize);
			
			foreach (Control control in Children)
                if (control.Enabled && control.Visible)
                    control.Draw(spriteBatch, gameTime);
        }

		/// <summary>
		/// Draw background.
		/// </summary>
		public void DrawRectangle(SpriteBatch spriteBatch, Rectangle locationSize, Color color)
		{		
			spriteBatch.Draw(TextureUtils.WhiteTexture, locationSize, null, color);
		}			
        
        /// <summary>
        /// Get selectable controls.
        /// </summary>        
        internal void GetSelectableControls(List<Control> controls)
        {
            if (IsSelectAble)
                controls.Add(this);

            foreach (Control control in Children)            
                control.GetSelectableControls(controls);            
        }
	}    
}
