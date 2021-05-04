using Microsoft.Xna.Framework;

using PolyOne.Scenes;
using PolyOne.Engine;
using PolyOne.Utility;

using PLWing.Platforms;
using PolyOne.LevelProcessor;
using PolyOne.HitboxProcessor;

namespace PLWing
{
    public enum GameTags
    {
        None = 0,
        Player = 1,
        AirEnemy = 2,
        Solid = 3,
        Empty = 4,
        Bullet = 5
    }

    public class Level : Scene
    {
        Player player;

        LevelTilesSolid tilesSolid;
        LevelTilesEmpty tilesEmpty;

        public LevelTiler Tile { get; private set; }
        LevelData levelData = new LevelData();
        HitboxData hitboxData = new HitboxData();

        bool[,] collisionInfoSolid;
        bool[,] collisionInfoEmpty;

        public override void LoadContent()
        {
            base.LoadContent();

            Tile = new LevelTiler();

            hitboxData = Engine.Instance.Content.Load<HitboxData>("SOMETHING");

            levelData = Engine.Instance.Content.Load<LevelData>("level");
            Tile.LoadContent(levelData);

            collisionInfoSolid = LevelTiler.TileConverison(Tile.CollisionLayer, 2);
            tilesSolid = new LevelTilesSolid(collisionInfoSolid);
            this.Add(tilesSolid);

            collisionInfoEmpty = LevelTiler.TileConverison(Tile.CollisionLayer, 0);
            tilesEmpty = new LevelTilesEmpty(collisionInfoEmpty);
            this.Add(tilesEmpty);

            player = new Player(new Vector2(100, 100));
            this.Add(player);
            player.Added(this);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            Engine.BeginParallax(player.Camera.TransformMatrix);
            Tile.DrawImageBackground(player.Camera.Position);
            Engine.End();

            Engine.Begin(player.Camera.TransformMatrix);
            Tile.DrawBackground();
            base.Draw();
            Engine.End();
        }
    }
}
