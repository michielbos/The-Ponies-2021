using System.Collections.Generic;
using System.Linq;
using Model.Data;
using Model.Ponies;

namespace Model.Actions {

public abstract class PonyAction {
    public readonly string identifier;
    public readonly Pony pony;
    public readonly string name;
    public bool active;
    public bool canceled;
    public bool finished;
    public int tickCount;
    public readonly IDictionary<string, object> data = new Dictionary<string, object>();

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
            data[pair.key] = pair.GetValue(property);
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
        if (Tick()) {
            Finish();
        }
    }
    
    /// <summary>
    /// Called every ingame second to execute this action's behaviour.
    /// Execution will start after the action becomes active.
    /// The frequency is dependent on the game speed and execution will pause when the game is paused.
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
    
    protected virtual void OnFinish() {
        // Override for additional finish behaviour.
    }

    public abstract PonyActionData GetData();

    /// <summary>
    /// Get the additional action data, converted to a DataPair array.
    /// This is used by the different action type implementations to save the additional action data.
    /// </summary>
    /// <returns></returns>
    protected DataPair[] GetDataPairs() {
        return data.Select(pair => DataPair.FromValues(pair.Key, pair.Value)).ToArray();
    }
}

}