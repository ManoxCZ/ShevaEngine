using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Control.
	/// </summary> 	
	public abstract class Control : IDisposable
    {
		protected readonly Log Log;
		protected ControlFlag Flags { get; set; }
		protected List<IDisposable> Disposables { get; }
        public BehaviorSubject<Color> BackColor { get; }
        public BehaviorSubject<Brush> Background { get; }
        //public string BackgroundImage { get; set; }
        //public Stretch BackgroundStretch { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
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
		public Margin Margin { get; set; }        
        public int GridRow { get; set; } = 0;
        public int GridColumn { get; set; } = 0;
		private Texture2D _whiteTexture;
		//private Texture2D _backgroundTexture;		
		public Subject<(InputState InputState, int X, int Y)> Click { get; }
		public Subject<(InputState InputState, int X, int Y)> MouseMove { get; }
		public Subject<(InputState InputState, int Wheel)> MouseWheel { get; }		 
		private int _previousMouseX = 0;
		private int _previousMouseY = 0;
		private int _previousWheel = 0;
		public SortedDictionary<ControlFlag, ControlAnimations> Animations { get; }		


		/// <summary>
		/// Constructor.
		/// </summary>
		public Control()
		{
			Log = new Log(GetType());

			Flags = ControlFlag.Default;
			Disposables = new List<IDisposable>();
			BackColor = CreateMember(Color.Transparent);
            Background = CreateMember<Brush>(null);
			//BackgroundImage = null;
			//BackgroundStretch = Stretch.Uniform;
			Children = new List<Control>();
			HorizontalAlignment = HorizontalAlignment.Center;
			VerticalAlignment = VerticalAlignment.Center;
			Enabled = true;
			IsSelectAble = false;
			IsSelected = false;
			Margin = new Margin();
			Name = GetType().Name;
			Visible = true;			
			Click = new Subject<(InputState InputState, int X, int Y)>();
			MouseMove = new Subject<(InputState InputState, int X, int Y)>();
			MouseWheel = new Subject<(InputState InputState, int Wheel)>();
			Animations = new SortedDictionary<ControlFlag, ControlAnimations>();

			foreach (ControlFlag flag in Enum.GetValues(typeof(ControlFlag)))
				Animations.Add(flag, new ControlAnimations());
		}

        /// <summary>
        /// Method creates new member.
        /// </summary>
        protected BehaviorSubject<T> CreateMember<T>(T value)
        {
            BehaviorSubject<T> instance = new BehaviorSubject<T>(value);

            Disposables.Add(instance);

            return instance;
        }

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{
			foreach (IDisposable item in Disposables)
				item.Dispose();

			Disposables.Clear();
            
			Click.Dispose();
			MouseMove.Dispose();
			MouseWheel.Dispose();					
		}

		/// <summary>
		/// Load content.
		/// </summary>        
		public virtual void LoadContent(ContentManager contentManager)
        {
			_whiteTexture = contentManager.Load<Texture2D>(@"Content\Graphics\White");

            Disposables.Add(Background.Subscribe(item =>
            {
                item?.LoadContent(contentManager);
            }));

			//if (!string.IsNullOrEmpty(BackgroundImage))
   //             _backgroundTexture = contentManager.Load<Texture2D>(BackgroundImage);					    

            foreach (Control child in Children)
                child.LoadContent(contentManager);
        }        

        /// <summary>
        /// Method resize control.
        /// </summary>
        public virtual void Resize(Rectangle locationSize)
        {
            LocationSize = new Rectangle(
				locationSize.X + Margin.Left,
				locationSize.Y + Margin.Bottom,
				locationSize.Width - (Margin.Left + Margin.Right),
				locationSize.Height - (Margin.Top + Margin.Bottom));

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

			
            if (BackColor.Value != Color.Transparent)
                DrawRectangle(spriteBatch, LocationSize, BackColor.Value);

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
			spriteBatch.Draw(_whiteTexture, locationSize, null, color);
		}				
	}    
}
