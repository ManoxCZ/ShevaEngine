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

                grid.RowDefinitions.OnNext(new[] {
                    new RowDefinition { Units = Units.Absolute, Value = logosSize },
                    new RowDefinition { Units = Units.Relative, Value = 2 },
                    new RowDefinition { Units = Units.Relative, Value = 2 },
                    new RowDefinition { Units = Units.Relative, Value = 3 },
                    new RowDefinition { Units = Units.Absolute, Value = logosSize }});


                Image splashLogo = new Image();
                splashLogo.GridRow.OnNext(2);
                splashLogo.Brush.OnNext(new ImageBrush(@"Content/Graphics/Splash"));
                splashLogo.Margin.OnNext(new Margin(4));
                
                grid.Children.Add(splashLogo);

                Label loadingLabel = new Label();
                loadingLabel.FontSize.OnNext(FontSize.Size20);
                loadingLabel.GridRow.OnNext(3);
                //loadingLabel.ForeColor.OnNext(Color.White);
                loadingLabel.HorizontalAlignment.OnNext(HorizontalAlignment.Center);
                loadingLabel.VerticalAlignment.OnNext(VerticalAlignment.Bottom);
                loadingLabel.Margin.OnNext(new Margin(4));

                //loadingLabel.Animations[ControlFlag.Default].AddRange(new[]
                //{
                //    new PulseColorAnimation(loadingLabel.ForeColor, new Color(0,0,0,255), new Color(200,200,200,255), 5),
                //});

                loadingLabel.Text.OnNext("Loading ...");

                grid.Children.Add(loadingLabel);

                Grid bottomGrid = new Grid();
                bottomGrid.GridRow.OnNext(4);
                bottomGrid.ColumnDefinitions.OnNext(new[] {
                    new ColumnDefinition { Units = Units.Absolute, Value = logosSize },
                    new ColumnDefinition { Units = Units.Relative, Value = 1 },
                    new ColumnDefinition { Units = Units.Absolute, Value = 256 }});

                Image monogameLogo = new Image();
                monogameLogo.Brush.OnNext(new ImageBrush(@"Content/Graphics/Monogame"));
                monogameLogo.Margin.OnNext(new Margin(4));                

                bottomGrid.Children.Add(monogameLogo);


                Label versionLabel = new Label();
                versionLabel.GridColumn.OnNext(2);
                //versionLabel.ForeColor.OnNext(Color.White);
                versionLabel.HorizontalAlignment.OnNext(HorizontalAlignment.Right);
                versionLabel.VerticalAlignment.OnNext(VerticalAlignment.Bottom);
                versionLabel.Margin.OnNext(new Margin(4));

                versionLabel.Text.OnNext($"Version: {Version.GetVersion()}");

                bottomGrid.Children.Add(versionLabel);

                grid.Children.Add(bottomGrid);
            }

            Layers.Add(layer);            

            Disposables.Add(game.Settings.Resolution.Subscribe((item) =>
            {
                layer.OnWindowResize(item.Width, item.Height);
            }));
        }
    }
}