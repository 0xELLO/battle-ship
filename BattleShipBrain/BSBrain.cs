using System;
using System.Collections.Generic;
using System.Text.Json;

namespace BattleShipBrain
{
    public class BSBrain
    {
        public readonly GameBoard[] GameBoards = new GameBoard[2];
        public GameConfiguration CurrentGameConfiguration;
        public GamePhase GamePhase = GamePhase.Initialized;
        public string GameName = "";
        public int PlayerToMove  = 0;
        public int MoveNumber = 1;
        
        public BSBrain(GameConfiguration configuration)
        {
            CurrentGameConfiguration = configuration;
            
            GameBoards[0] = new GameBoard();
            GameBoards[1] = new GameBoard();
            
            GameBoards[0].Board = new BoardSquareState[configuration.BoardSizeX, configuration.BoardSizeY];
            GameBoards[1].Board = new BoardSquareState[configuration.BoardSizeX, configuration.BoardSizeY];

            GameBoards[0].Rule = configuration.EShipTouchRule;
            GameBoards[1].Rule = configuration.EShipTouchRule;
        }

        public void PlaceBomb(int x, int y)
        {
            SwitchPlayer();
            GameBoards[PlayerToMove].Board[x, y].IsBomb = true;
            var ship = GameBoards[PlayerToMove].Ships.Find(ship => ship.Coordinates.Exists
                (c => c.X == x && c.Y == y));
            if (ship != null)
            {
                if (ship!.IsShipSunk(GameBoards[PlayerToMove].Board))
                {
                    foreach (var shipCoordinate in ship.Coordinates)
                    {
                        GameBoards[PlayerToMove].Board[shipCoordinate.X, shipCoordinate.Y].IsSunk = true;
                    }
                };
                SwitchPlayer();
            }
        }
        
        // Return true if ship placement succeeded 
        public bool PlaceShip(string name, List<Coordinate> coordinates, int length, int width)
        {
            return GameBoards[PlayerToMove].AddShip(name, coordinates);
        }

        public BoardSquareState[,] GetCurrentPlayerBoard()
        {
            return GameBoards[PlayerToMove].Board;
        }  
        
        public BoardSquareState[,] GetOppositePlayerBoard()
        {
            SwitchPlayer();
            var b =  GameBoards[PlayerToMove].Board;
            SwitchPlayer();
            return b;
        }

        public bool CheckIfPlayerWon()
        {
            MoveNumber++;
            SwitchPlayer();
            if (!GameBoards[PlayerToMove].CheckIfLost())
            {
                SwitchPlayer();
                return false;
            }
            SwitchPlayer();
            return true;
        }

        public void SwitchPlayer()
        {
            switch (PlayerToMove)
            {
                case 0:
                    PlayerToMove = 1;
                    break;
                case 1:
                    PlayerToMove = 0;
                    break;
            }
        }

        public void CreateRandomBoards()
        {
            FindRandomBoardSolution(0, 0);
            FindRandomBoardSolution(1, 0);
        }

        private bool FindRandomBoardSolution(int gameBoardIndex, int depth)
        {
            if (depth == 100) return false;
            
            var rd = new Random();
            GameBoards[gameBoardIndex] = new GameBoard
            {
                Board = new BoardSquareState[CurrentGameConfiguration.BoardSizeX, CurrentGameConfiguration.BoardSizeY],
                Rule = CurrentGameConfiguration.EShipTouchRule
            };

            foreach (var shipConfig in CurrentGameConfiguration.ShipConfigs)
            {
                for (var i = 0; i < shipConfig.Quantity; i++)
                {
                    // trys to place ship 5 times, on 5th starts over 
                    for (int tryA = 0; tryA < 5; tryA++)
                    {
                        var coordinates = new List<Coordinate>();

                        var x = rd.Next(0, CurrentGameConfiguration.BoardSizeX);
                        var y = rd.Next(0, CurrentGameConfiguration.BoardSizeY);
                            
                        for (var sizeY = 0; sizeY < shipConfig.ShipSizeY; sizeY++)
                        {
                            for (var sizeX = 0; sizeX < shipConfig.ShipSizeX; sizeX++)
                            {
                                coordinates.Add(new Coordinate
                                {
                                    X = x + sizeX,
                                    Y = y + sizeY
                                });
                            }
                        }
                        var success = GameBoards[gameBoardIndex].AddShip(shipConfig.Name, coordinates);
                        if (success) break;
                        if (tryA == 4)
                        {
                            return FindRandomBoardSolution(gameBoardIndex, depth++);
                        }
                    }
                }
            }
            // solution found
            return true;
        }
        

        private BoardSquareState[,] CopyOfBoard(BoardSquareState[,] board)
        {
            var res = new BoardSquareState[board.GetLength(0),board.GetLength(1)];
            
            for (var x = 0; x < board.GetLength(0); x++)
            {
                for (var y = 0; y < board.GetLength(1); y++)
                {
                    res[x, y] = board[x, y];
                }
            }
            return res;
        }
        
        public string GetBrainJson()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var dto = new SaveGameDTO();
            dto.SetGameBoard(GameBoards);
            dto.CurrentPlayerNo = PlayerToMove;
            dto.MovesN = MoveNumber;
            var jsonStr = JsonSerializer.Serialize(dto, jsonOptions);
            return jsonStr;
        }


        public void RestoreBrainFromJson(string json)
        {
            var dto = JsonSerializer.Deserialize<SaveGameDTO>(json);
            if (dto == null) return;
            
            var restoredGameBoards = dto.GetGameBoard();
            GameBoards[0] = restoredGameBoards[0];
            GameBoards[1] = restoredGameBoards[1];
            PlayerToMove = dto.CurrentPlayerNo;
            MoveNumber = dto.MovesN;

            var shipConfigs = new HashSet<ShipConfig>();
            
            foreach (var ship in GameBoards[0].Ships)
            {
                shipConfigs.Add(new ShipConfig
                {
                    Name = ship.Name,
                    Quantity = GameBoards[0].Ships.FindAll(s => s.Name == ship.Name).Count,
                    ShipSizeX = ship.GetShipSize().Item1,
                    ShipSizeY = ship.GetShipSize().Item2
                });
            }

            CurrentGameConfiguration = new GameConfiguration
            {
                BoardSizeX = GameBoards[0].Board.GetLength(0),
                BoardSizeY = GameBoards[0].Board.GetLength(1),
                EShipTouchRule = GameBoards[0].Rule,
                ShipConfigs = new List<ShipConfig>(shipConfigs)
            };
        }

        public (string, string) GetGameBoardsAsString()
        {
            var dto = new SaveGameDTO();
            dto.SetGameBoard(GameBoards);
            var firstBoard = dto.GameBoards[0];
            var secondBoard = dto.GameBoards[1];
            
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var jsonFirstBoard = JsonSerializer.Serialize(firstBoard, jsonOptions);
            var jsonSecondBoard = JsonSerializer.Serialize(secondBoard, jsonOptions);

            return (jsonFirstBoard, jsonSecondBoard);
        }

        public void RestoreBoardsFromString(string firstBoard, string secondBoard)
        {
            var restoredFirstBoard = JsonSerializer.Deserialize<SaveGameDTO.GameBoardDTO>(firstBoard);
            var restoredSecondBoard = JsonSerializer.Deserialize<SaveGameDTO.GameBoardDTO>(secondBoard);

            var dto = new SaveGameDTO();
            dto.GameBoards[0] = restoredFirstBoard!;
            dto.GameBoards[1] = restoredSecondBoard!;
            
            var restoredGameBoards = dto.GetGameBoard();
            GameBoards[0] = restoredGameBoards[0];
            GameBoards[1] = restoredGameBoards[1];

            GameBoards[0].Rule = CurrentGameConfiguration.EShipTouchRule;
            GameBoards[1].Rule = CurrentGameConfiguration.EShipTouchRule;
        }

        public int GetPlayerN()
        {
            return PlayerToMove;
        }

        public int GetMoveN()
        {
            return MoveNumber;
        }
    }
}