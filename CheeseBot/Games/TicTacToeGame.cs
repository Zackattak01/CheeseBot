using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Disqord;

namespace CheeseBot.Games
{
    public class TicTacToeGame
    {
        public const string AcceptId = "accept";
        public const string DenyId = "deny";
        
        private readonly Random _random;
        private TicTacToePlayerState[] _board;
        private TicTacToePlayerState _currentPlayer;
        private bool _gameOver = false;

        public Snowflake MessageId { get; }
        public Snowflake ChallengerId { get;  }
        public Snowflake OpponentId { get; }
        
        public Snowflake XPlayerId { get; private set; }
        public Snowflake OPlayerId { get; private set; }

        public Snowflake CurrentPlayerId => _currentPlayer == TicTacToePlayerState.XPlayer ? XPlayerId : OPlayerId;
        public bool IsAccepted { get; private set; } = false;
        
        public TicTacToeGame(Snowflake messageId, Snowflake challengerId, Snowflake opponentId, Random random)
        {
            _random = random;
            MessageId = messageId;
            ChallengerId = challengerId;
            OpponentId = opponentId;
        }

        public void Accept()
        {
            IsAccepted = true;
            _board = new TicTacToePlayerState[9];
            
            for (var i = 0; i < _board.Length; i++)
                _board[i] = TicTacToePlayerState.None;

            var playerDecider = _random.Next(0, 2);

            if (playerDecider == 0)
            {
                XPlayerId = ChallengerId;
                OPlayerId = OpponentId;
            }
            else
            {
                XPlayerId = OpponentId;
                OPlayerId = ChallengerId;
            }

            _currentPlayer = TicTacToePlayerState.XPlayer;
        }

        public IEnumerable<LocalRowComponent> GenerateGameBoardComponent()
        {
            if (_board is null)
                throw new Exception("Board was not initialized properly");
            
            var actionRows = new LocalRowComponent[3];
                        
            for (var i = 0; i < actionRows.Length; i++)
            {
                actionRows[i] = new LocalRowComponent();
                for (var j = 0; j < 3; j++)
                {
                    var boardPos = (i * 3) + j;
                    var label = _board[boardPos] switch
                    {
                        TicTacToePlayerState.None => "\u200b",
                        TicTacToePlayerState.XPlayer => "X",
                        TicTacToePlayerState.OPlayer => "O",
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    
                    actionRows[i].AddComponent(new LocalButtonComponent
                    {
                        CustomId = boardPos.ToString(),
                        Label = label,
                        Style = ButtonComponentStyle.Secondary,
                        IsDisabled = label != "\u200b" || _gameOver
                    });
                }
            }

            return actionRows;
        }

        public bool MakeMove(Snowflake playerId, int boardPos)
        {
            var currentPlayerId = _currentPlayer switch
            {
                TicTacToePlayerState.XPlayer => XPlayerId,
                TicTacToePlayerState.OPlayer => OPlayerId,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (playerId != currentPlayerId)
                return false;

            if (_board[boardPos] != TicTacToePlayerState.None)
                return false;

            _board[boardPos] = _currentPlayer;
            _currentPlayer = _currentPlayer == TicTacToePlayerState.XPlayer
                ? TicTacToePlayerState.OPlayer
                : TicTacToePlayerState.XPlayer;

            return true;
        }

        public bool CheckGameOver(out Snowflake winner)
        {
            if (CheckWin(TicTacToePlayerState.XPlayer))
            {
                winner = XPlayerId;
                _gameOver = true;
                return true;
            }
            else if (CheckWin(TicTacToePlayerState.OPlayer))
            {
                winner = OPlayerId;
                _gameOver = true;
                return true;
            }
            else if (_board.All(x => x != TicTacToePlayerState.None))
            {
                winner = default;
                _gameOver = true;
                return true;
            }
            else
            {
                winner = default;
                return false;
            }
        }

        private bool CheckWin(TicTacToePlayerState player)
        {
            for (var i = 0; i < 3; i++)
            {
                var topRow = i;
                var middleRow = topRow + 3;
                var bottomRow = middleRow + 3;

                if (_board[topRow] == player &&
                    _board[middleRow] == player &&
                    _board[bottomRow] == player)
                    return true;

                var leftColumn = i * 3;
                var middleColumn = leftColumn + 1;
                var rightColumn = middleColumn + 1;
                
                if (_board[leftColumn] == player &&
                    _board[middleColumn] == player &&
                    _board[rightColumn] == player)
                    return true;
            }
            
            if (_board[0] == player &&
                _board[4] == player &&
                _board[8] == player)
                return true;

            if (_board[2] == player &&
                _board[4] == player &&
                _board[6] == player)
                return true;

            return false;
        }
    }
}