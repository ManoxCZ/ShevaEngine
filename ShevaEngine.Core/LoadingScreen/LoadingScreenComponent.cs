using Microsoft.Xna.Framework;
using ShevaEngine.UI;
using System;


namespace ShevaEngine.Core
{
    /// <summary>
    /// Loading screen component.
    /// </summary>
    public class LoadingScreenComponent : ShevaGameComponent
    {
        public const int logosSize = 80;

        /// <summary>
		/// Initialize.
		/// </summary>
		public override void LoadContent(ShevaGame game)
        {
            base.LoadContent(game);

            Layer layer = new Layer();
            layer.IsActive.OnNext(true);

            if (layer.Control is Grid grid)
            {
                grid.IsSelectAble = true;
                grid.Background.OnNext(new ImageBrush(@"Content/Graphics/LoadingScreen", Stretch.UniformToFill));

                grid.RowDefinitions.Clear();
                grid.RowDefinitions.Add(new GridRowDefinition { Units = Units.Absolute, Height = logosSize });
                grid.RowDefinitions.Add(new GridRowDefinition { Units = Units.Relative, Height = 2 });
                grid.RowDefinitions.Add(new GridRowDefinition { Units = Units.Relative, Height = 2 });
                grid.RowDefinitions.Add(new GridRowDefinition { Units = Units.Relative, Height = 3 });
                grid.RowDefinitions.Add(new GridRowDefinition { Units = Units.Absolute, Height = logosSize });


                Image splashLogo = new Image();
                splashLogo.GridRow = 2;
                splashLogo.Brush.OnNext(new ImageBrush(@"Content/Graphics/Splash"));
                splashLogo.Margin = new Margin()
                {
                    Left = 4,
                    Right = 4,
                    Top = 4,
                    Bottom = 4
                };

                grid.Children.Add(splashLogo);

                Label loadingLabel = new Label();
                loadingLabel.FontSize.OnNext(FontSize.Size20);
                loadingLabel.GridRow = 3;
                loadingLabel.ForeColor.OnNext(Color.White);
                loadingLabel.HorizontalAlignment = HorizontalAlignment.Center;
                loadingLabel.VerticalAlignment = VerticalAlignment.Bottom;
                loadingLabel.Margin = new Margin()
                {
                    Left = 4,
                    Right = 4,
                    Top = 4,
                    Bottom = 4
                };

                loadingLabel.Animations[ControlFlag.Default].AddRange(new[]
                {
                    new PulseColorAnimation(loadingLabel.ForeColor, new Color(0,0,0,255), new Color(200,200,200,255), 5),
                });

                loadingLabel.Text.OnNext("Loading ...");

                grid.Children.Add(loadingLabel);

                Grid bottomGrid = new Grid();
                bottomGrid.GridRow = 4;
                bottomGrid.ColumnDefinitions.Clear();
                bottomGrid.ColumnDefinitions.Add(new GridColumnDefinition { Units = Units.Absolute, Width = logosSize });
                bottomGrid.ColumnDefinitions.Add(new GridColumnDefinition { Units = Units.Relative, Width = 1 });
                bottomGrid.ColumnDefinitions.Add(new GridColumnDefinition { Units = Units.Absolute, Width = 256 });

                Image monogameLogo = new Image();
                monogameLogo.Brush.OnNext(new ImageBrush(@"Content/Graphics/Monogame"));
                monogameLogo.Margin = new Margin()
                {
                    Left = 4,
                    Right = 4,
                    Top = 4,
                    Bottom = 4
                };

                bottomGrid.Children.Add(monogameLogo);


                Label versionLabel = new Label();
                versionLabel.GridColumn = 2;
                versionLabel.ForeColor.OnNext(Color.White);
                versionLabel.HorizontalAlignment = HorizontalAlignment.Right;
                versionLabel.VerticalAlignment = VerticalAlignment.Bottom;
                versionLabel.Margin = new Margin()
                {
                    Left = 4,
                    Right = 4,
                    Top = 4,
                    Bottom = 4
                };

                versionLabel.Text.OnNext($"Version: {Version.GetVersion()}");

                bottomGrid.Children.Add(versionLabel);

                grid.Children.Add(bottomGrid);
            }

            Layers.Add(layer);

            layer.LoadContent(game);

            Disposables.Add(game.Settings.Resolution.Subscribe((item) =>
            {
                layer.OnWindowResize(item.Width, item.Height);
            }));
        }
    }
}