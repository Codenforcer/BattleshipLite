﻿using BattleshipLibrary.Models;
using BattleshipLogic;
using BattleshipLogic.Models;
using System;

namespace ConsoleUI
{
    public class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();

            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");

            PlayerInfoModel winner = null;

            do
            {

                DisplayShotGrid(activePlayer);

                RecordPlayerShot(activePlayer, opponent);

                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);


                if (doesGameContinue == true)
                {
                    (activePlayer, opponent) = (opponent, activePlayer); // Use Tuple to swap positions.
                }
                else
                {
                    winner = activePlayer;
                }

            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations to { winner.UsersName } for winning!");
            Console.WriteLine($"{ winner.UsersName} took { GameLogic.GetShotCount(winner) } shots.");
        }

        // Ask activePlayer for a shot.
        // Determine if it is a valid shot - keep looping if it is not.
        // Determine shot results.
        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int column = 0;

            do
            {
                string shot = AskForShot();
                (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                isValidShot = GameLogic.ValidateShot(activePlayer, row, column);

                if (isValidShot == false)
                {
                    Console.WriteLine("Invalid shot location, please try again.");
                }

            } while (isValidShot == false);

            bool isAHit = GameLogic.IdentifyShotResult(opponent, row, column);

            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);
        }

        private static string AskForShot()
        {
            Console.Write("Please enter your shot selection: ");
            string output = Console.ReadLine();
            return output;
        }

        // Display grid from activePlayer on where they fired.
        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                }


                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" { gridSpot.SpotLetter }{ gridSpot.SpotNumber } ");

                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O ");
                }
                else
                {
                    Console.Write(" ? ");
                }
            }
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship Lite");
            Console.WriteLine("Created by Luke Wardle");
            Console.WriteLine("Part of the Tim Corey C# Masterclass");
            Console.WriteLine();

        }

        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();

            Console.WriteLine($"Player information for {playerTitle}");

            // Ask the user for their name.
            output.UsersName = AskForUsersName();

            // Load up the shot grid.
            GameLogic.InitializeGrid(output);

            // Ask the user for their ship placement.
            PlaceShips(output);

            // Clear.
            Console.Clear();

            return output;
        }

        private static string AskForUsersName()
        {
            Console.Write("What is your name: ");
            string output = Console.ReadLine();

            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            // Hardcoded.
            do
            {
                Console.Write($"Where do you want to place ship number {model.ShipLocations.Count + 1}: ");
                string location = Console.ReadLine();

                bool isValidLocation = GameLogic.PlaceShip(model, location);

                if (isValidLocation == false)
                {
                    Console.WriteLine("That was not a valid location. Please try again.");
                }

            } while (model.ShipLocations.Count < 5);
        }
    }
}
