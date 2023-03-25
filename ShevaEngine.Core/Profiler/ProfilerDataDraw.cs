using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms.VisualStyles;

namespace ShevaEngine.Core.Profiler;

internal class ProfilerDataDraw
{
    private const int MARGIN = 5;
    private const int ROW_HEIGHT = 25;
    private const int INDENT = 25;

    private ProfilerService _profilerService = null!;
    private SpriteBatch _spriteBatch = null!;
    private SpriteFont _spriteFont = null!;

    
    public void LoadContent(ShevaGame game)
    {
        if (ShevaServices.GetService<ProfilerService>() is ProfilerService profilerService)
        {
            _profilerService = profilerService;
        }

        _spriteBatch = new SpriteBatch(game.GraphicsDevice);
        _spriteFont = game.Content.Load<SpriteFont>(@"Content\\Fonts\\Arial");
    }

    public void Draw()
    {
        _spriteBatch.Begin();

        int row = 0;
        int indent = 0;

        foreach (var item in _profilerService.RootNode.Children)
        {
            DrawProfilerNode(item, ref row, ref indent);
        }

        _spriteBatch.End();
    }


    private void DrawProfilerNode(ProfilerScopeResult nodeResult, ref int row, ref int indent)
    {
        _spriteBatch.DrawString(_spriteFont, 
            $"{nodeResult.Name} - {nodeResult.Time.ToString("0.00")}[ms]", 
            new Vector2(MARGIN + indent * INDENT, MARGIN + row * ROW_HEIGHT), 
            Color.White);

        row++;

        indent++;

        foreach (var item in nodeResult.Children)
        {
            DrawProfilerNode(item, ref row, ref indent);
        }

        indent--;        
    }
}
