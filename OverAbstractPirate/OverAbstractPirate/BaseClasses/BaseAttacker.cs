﻿using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;


namespace MyBot
{
    public abstract class BaseAttacker : ICommand
    {
        private Pirate pirate;
        private Pirate targetEnemy;
        private Location destination;
        private Location positionInFormation;


        public Pirate Pirate { get => pirate; set => pirate = value; }
        public Pirate TargetEnemy { get => targetEnemy; set => targetEnemy = value; }
        public Location Destination { get => destination; set => destination = value; }
        public Location PositionInFormation { get => positionInFormation; set => positionInFormation = value; }

        public abstract void ExecuteCommand();
        public abstract void SailToPosition();
        public void SailToTarget()
        {
            if (!this.AttackersTryPush())
            {
                this.Pirate.Sail(Destination);
            }
        }

        #region AttackersTryPush
        public bool AttackersTryPush()
        {
            Location upperDot = this.pirate.Location.Towards(Destination, pirate.PushRange);
            Location U = upperDot.Subtract(this.pirate.GetLocation());

            foreach (Pirate enemy in GameSettings.Game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (this.pirate.CanPush(enemy))
                {
                   
                    this.pirate.Push(enemy, pirate.Location.Add(U.Multiply(-5)));
                    
                    GameSettings.Game.Debug("pirate " + this.pirate + " pushes " + enemy + " towards " + enemy.InitialLocation);
                    //Did push.
                    return true;
                }
            }
            // Didn't push.
            return false;
        }
        #endregion

        #region TargetedPushing
        /// <summary>
        /// takes a carrier for perspective to prtoect from enemys coming
        /// an ataacker who choose to targetPush ill lock himself on to a target and face it until it can push it.  
        /// </summary>
        /// <param name="carrier"></param>
        public void TargetedPushing(Carrier carrier)
        {
            Location upperDot = this.pirate.Location.Towards(Destination, pirate.PushRange);
            Location U = upperDot.Subtract(this.pirate.GetLocation());

            if (this.pirate.CanPush(targetEnemy))
                this.pirate.Push(targetEnemy, pirate.Location.Add(U.Multiply(-5)));
            else
                this.pirate.Sail(carrier.pirate.Location.Towards(targetEnemy, this.pirate.PushRange*2));
        }
        #endregion
    }
}