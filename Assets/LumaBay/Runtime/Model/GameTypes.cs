using System;
using System.Collections.Generic;
using UnityEngine;

namespace LumaBay
{
    public enum PieceKind
    {
        None = -1,
        Shell = 0,
        Starfish = 1,
        Lantern = 2,
        Compass = 3,
        Crystal = 4,
        Flower = 5
    }

    public enum SpecialKind
    {
        None = 0,
        ClearRow = 1,
        ClearColumn = 2,
        Bomb = 3,
        Rainbow = 4
    }

    public enum ObstacleKind
    {
        None = 0,
        Crate = 1,
        Ice = 2,
        Net = 3
    }

    [Serializable]
    public sealed class BoardCell
    {
        public PieceKind Piece = PieceKind.None;
        public SpecialKind Special = SpecialKind.None;
        public ObstacleKind Obstacle = ObstacleKind.None;
        [SerializeField] private int obstacleLayers;
        public int FogLayers;

        public int ObstacleLayers
        {
            get => obstacleLayers;
            set
            {
                bool crateAbsorbedHit = Obstacle == ObstacleKind.Crate && obstacleLayers > 0 && value < obstacleLayers;
                obstacleLayers = Mathf.Max(0, value);
                if (crateAbsorbedHit)
                {
                    Piece = PieceKind.None;
                    Special = SpecialKind.None;
                }
            }
        }

        public bool IsEmpty => Piece == PieceKind.None;
        public bool SwapLocked => Obstacle == ObstacleKind.Net && ObstacleLayers > 0;

        public BoardCell Clone()
        {
            return new BoardCell
            {
                Piece = Piece,
                Special = Special,
                Obstacle = Obstacle,
                ObstacleLayers = ObstacleLayers,
                FogLayers = FogLayers
            };
        }
    }

    public sealed class MatchGroup
    {
        public readonly List<Vector2Int> Cells = new List<Vector2Int>();
        public bool Horizontal;
    }

    public sealed class MoveResult
    {
        public bool Valid;
        public int Cascades;
        public int ClearedPieces;
        public int ClearedFog;
        public int ClearedObstacles;
        public readonly Dictionary<PieceKind, int> Collected = new Dictionary<PieceKind, int>();

        public void AddCollected(PieceKind kind)
        {
            if (kind == PieceKind.None)
            {
                return;
            }

            if (!Collected.ContainsKey(kind))
            {
                Collected[kind] = 0;
            }

            Collected[kind]++;
            ClearedPieces++;
        }
    }

    [Serializable]
    public sealed class LevelDefinition
    {
        public int Id;
        public int Width = 8;
        public int Height = 8;
        public int Moves = 24;
        public PieceKind TargetPiece = PieceKind.Shell;
        public int TargetCount = 12;
        public int FogCount;
        public int CrateCount;
        public int IceCount;
        public int NetCount;
        public int Seed = 1001;

        public string DifficultyLabel
        {
            get
            {
                if (Id <= 5) return "Easy";
                if (Id <= 15) return "Normal";
                if (Id <= 24) return "Hard";
                return "Expert";
            }
        }
    }
}
