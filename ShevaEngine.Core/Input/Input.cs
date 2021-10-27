using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Reactive.Subjects;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Input class.
    /// </summary>
    public class Input : IDisposable
    {
        private KeyboardState _previousKeyboardState;
        private int _keysCount;
        private Subject<Keys>[] _onKeyPressedSubjects;
        private BehaviorSubject<bool>[] _isKeyPressedSubjects;


        /// <summary>
        /// Constructor.
        /// </summary>
        public Input()
        {
            InitializeKeyboard();
        }

        /// <summary>
        /// Initialize keyboard.
        /// </summary>
        private void InitializeKeyboard()
        {
            _keysCount = (int)Enum.GetValues(typeof(Keys)).Cast<Keys>().Max() + 1;

            _onKeyPressedSubjects = new Subject<Keys>[_keysCount];
            _isKeyPressedSubjects = new BehaviorSubject<bool>[_keysCount];

            for (int i = 0; i < _keysCount; i++)
            {
                _onKeyPressedSubjects[i] = new Subject<Keys>();
                _isKeyPressedSubjects[i] = new BehaviorSubject<bool>(false);
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < _keysCount; i++)
            {
                _onKeyPressedSubjects[i].Dispose();
                _isKeyPressedSubjects[i].Dispose();
            }

            _onKeyPressedSubjects = null;
        }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update()
        {
            UpdateKeyboard();
        }

        /// <summary>
        /// Update keyboard.
        /// </summary>
        private void UpdateKeyboard()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            for (int i = 0; i < _keysCount; i++)
            {
                if (_isKeyPressedSubjects[i].Value != keyboardState.IsKeyDown((Keys)i))
                    _isKeyPressedSubjects[i].OnNext(keyboardState.IsKeyDown((Keys)i));

                if (keyboardState.IsKeyDown((Keys)i) != _previousKeyboardState.IsKeyDown((Keys)i))
                {
                    if (keyboardState.IsKeyUp((Keys)i))
                        _onKeyPressedSubjects[i].OnNext((Keys)i);
                }
            }

            _previousKeyboardState = keyboardState;
        }

        /// <summary>
        /// On key pressed.
        /// </summary>
        public Subject<Keys> OnKeyPressed(Keys key)
        {
            return _onKeyPressedSubjects[(int)key];
        }

        /// <summary>
        /// On key pressed.
        /// </summary>
        public BehaviorSubject<bool> IsKeyPressed(Keys key)
        {
            return _isKeyPressedSubjects[(int)key];
        }
    }
}
