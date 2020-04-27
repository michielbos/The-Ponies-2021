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
    public int tickCount;
    public readonly DataMap data = new DataMap();

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
}

}