using System;
using System.Collections.Generic;
using System.Linq;
using Model.Data;
using Model.Ponies;
using UnityEngine;

namespace Model.Actions {

/// <summary>
/// Base class for all queue actions that a pony can do.
/// </summary>
public abstract class PonyAction {
    public readonly string identifier;
    public readonly Pony pony;
    public readonly string name;
    public bool active;
    public bool canceled;
    public bool finished;
    public PonyActionTrigger trigger = PonyActionTrigger.External;
    public int tickCount;
    public readonly DataMap data = new DataMap();

    /// <summary>
    /// Whether this action is visible when clicking the object.
    /// This can be set to false for actions that are only triggered by autonomy or other non-player triggers.
    /// </summary>
    public virtual bool Visible => true;

    /// <summary>
    /// How attractive this action is for ponies following free will.
    /// 0 (or lower) means ponies will never do this action on their own.
    /// Values from 1 to 50 are only selected when free will is set to full (and by non-selected ponies).
    /// Values from 51 to 100 are selected in both full and minimal free will mode.
    /// Values above 100 are rounded down to 100.
    /// </summary>
    public virtual int AutonomyScore => 0;

    protected PonyAction(string identifier, Pony pony, string name) {
        this.identifier = identifier;
        this.pony = pony;
        this.name = name;
    }

    /// <summary>
    /// Add data pairs to this action's additional data.
    /// This is used for loading action data from a saved game.
    /// </summary>
    internal void AddDataPairs(DataPair[] dataPairs, Property.Property property) {
        foreach (DataPair pair in dataPairs) {
            data.Put(pair.key, pair.GetValue(property));
        }
    }

    internal void Load(int tickCount) {
        this.tickCount = tickCount;
        active = tickCount > 0;
    }

    internal void TickAction() {
        tickCount++;
        if (tickCount == 1) {
            OnStart();
        }
        if (!TargetExists()) {
            finished = true;
            OnTargetLost();
            return;
        }
        try {
            if (Tick()) {
                Finish();
            }
        } catch (Exception e) {
            Debug.LogError("Unhandled error on action: " + identifier);
            Debug.LogException(e);
            finished = true;
            OnForceCancel();
        }
    }

    protected internal abstract bool TargetExists();
    
    /// <summary>
    /// Called every ingame second to execute this action's behaviour.
    /// Execution will start after the action becomes active.
    /// The frequency is dependent on the game speed and execution will pause when the game is paused.
    /// Should return true when the action is done. False if the action is still in progress.
    /// </summary>
    public abstract bool Tick();

    internal void SetActive() {
        if (!active) {
            active = true;
            OnActivated();
        }
    }
    
    protected virtual void OnActivated() {
        // Override for additional activation behaviour.
    }
    
    protected virtual void OnStart() {
        // Override for additional first tick behaviour.
    }

    public void Cancel() {
        if (!canceled) {
            canceled = true;
            OnCancel();
        }
    }

    protected virtual void OnCancel() {
        // Override for additional cancel behaviour.
    }

    /// <summary>
    /// Interrupt this action because it was low priority and another action was queued by the player or another pony.
    /// Currently does the same as Cancel, but in a separate method in case we alter behaviour in the future.
    /// </summary>
    public void Interrupt() {
        Cancel();
    }

    protected void Finish() {
        if (!finished) {
            finished = true;
            OnFinish();
        }
    }
    
    /// <summary>
    /// Called when a pony has completed its action, regardless of whether it was successful, failed or canceled.
    /// If OnForceCancel hasn't been implemented, this is also called when an action is forcefully canceled.
    /// </summary>
    protected virtual void OnFinish() {
        // Override for additional finish behaviour.
    }

    /// <summary>
    /// Called when the target of the pony went missing (for example, was sold by the player).
    /// No more ticks will be called after this action.
    /// If not implemented, this method will call OnForceCancel instead. (which calls OnFinish when not implemented)
    /// </summary>
    protected virtual void OnTargetLost() {
        OnForceCancel();
    }
    
    /// <summary>
    /// Called when the action was forcefully canceled (for example, because of an unhandle exception or a missing target).
    /// No more ticks will be called after this action.
    /// If not implemented, this method will call OnFinish instead.
    /// </summary>
    protected virtual void OnForceCancel() {
        OnFinish();
    }

    public abstract PonyActionData GetData();

    /// <summary>
    /// Called when a next action was queued.
    /// </summary>
    public void NextActionQueued(PonyAction nextAction) {
        // Non-essential free will actions are interrupted by any non-free will actions.
        if (trigger == PonyActionTrigger.FreeWill && nextAction.trigger != PonyActionTrigger.FreeWill) {
            if (AutonomyScore < 51) {
                Interrupt();
            }
        }
    }
}

}