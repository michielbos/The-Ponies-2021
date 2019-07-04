using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Controllers.Playmode;
using JetBrains.Annotations;
using Model.Actions;
using Model.Actions.Actions;
using Model.Data;
using UnityEngine;

namespace Model.Ponies {

public class Pony: MonoBehaviour, ITimeTickListener, IActionProvider {
    private const float WalkSpeed = 3f;
    
    public GameObject indicator;
    public Material indicatorMaterial;
    public List<PonyAction> queuedActions = new List<PonyAction>();
    [CanBeNull] public PonyAction currentAction;
    
    public string ponyName;
    public PonyRace race;
    public Gender gender;
    public PonyAge age;
    public Needs needs;

    [CanBeNull] private Path walkPath;
    private Vector2Int? nextWalkTile;

    public bool IsSelected => HouseholdController.Instance.selectedPony == this;
    public bool IsWalking => nextWalkTile != null || walkPath != null;
    
    public Vector2Int TilePosition {
        get {
            Vector3 position = transform.position;
            return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        }
        set { transform.position = new Vector3(value.x, 0, value.y); }
    }

    public void Init(string ponyName, PonyRace race, Gender gender, PonyAge age, Needs needs) {
        this.ponyName = ponyName;
        this.race = race;
        this.gender = gender;
        this.age = age;
        this.needs = needs;
        indicator.GetComponent<Renderer>().material = new Material(indicatorMaterial);
    }

    private void OnEnable() {
        TimeController.Instance.AddTickListener(this);
    }

    private void OnDisable() {
        if (TimeController.HasInstance) {
            TimeController.Instance.RemoveTickListener(this);
        }
    }

    private void Update() {
        HandleWalking();
    }

    private void HandleWalking() {
        if (nextWalkTile == null && walkPath != null) {
            nextWalkTile = walkPath.NextTile();
        }
        if (nextWalkTile == null)
            return;
        Vector2Int nextTile = nextWalkTile.Value;
        Vector3 target = new Vector3(nextTile.x, 0, nextTile.y);
        float speed = WalkSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, speed);
        if (transform.position == target) {
            nextWalkTile = null;
            if (walkPath != null && !walkPath.HasNext()) {
                walkPath = null;
            }
        }
    }

    public void OnTick() {
        needs.ApplyDecay();
        if (currentAction == null && queuedActions.Count > 0) {
            currentAction = queuedActions[0];
            currentAction.SetActive();
            UpdateQueue();
            
        }
        if (currentAction != null) {
            currentAction.TickAction();
            if (currentAction.finished) {
                queuedActions.Remove(currentAction);
                currentAction = null;
                UpdateQueue();
            }
        }
    }

    public PonyData GetPonyData() {
        return new PonyData(ponyName, (int) race, (int) gender, (int) age, needs.GetNeedsData());
    }

    public void SetSelected(bool selected) {
        indicator.SetActive(selected);
    }

    public void QueueAction(PonyAction action) {
        queuedActions.Add(action);
        if (IsSelected) {
            SoundController.Instance.PlaySound(SoundType.QueueAdded);
            UpdateQueue();
        }
    }
    
    public List<PonyAction> GetActions(Pony pony) {
        if (pony == this) {
            return new List<PonyAction>() {
                new FakeAction(pony, "Why am I a cube?")
            };
        }
        return new List<PonyAction>() {
            new SocialAction(pony, "Chat", this),
            new SocialAction(pony, "Slap", this),
            new SocialAction(pony, "Flirt", this),
        };
    }

    public void CancelAction(PonyAction action) {
        if (action == currentAction) {
            action.Cancel();
        } else {
            action.canceled = true;
            action.finished = true;
            queuedActions.Remove(action);
        }
        if (IsSelected) {
            SoundController.Instance.PlaySound(SoundType.QueueRemoved);
            UpdateQueue();
        }
    }

    private void UpdateQueue() {
        if (IsSelected) {
            HouseholdController.Instance.actionQueue.UpdateActions(queuedActions);
        }
    }

    public bool SetWalkTarget(Vector2Int target) {
        walkPath = Pathfinding.PathToTile(TilePosition, target);
        return walkPath != null;
    }
    
    /// <summary>
    /// Set the walk target to the nearest of the provided tiles.
    /// </summary>
    public bool SetWalkTargetToNearest(IEnumerable<Vector2Int> targets) {
        walkPath = Pathfinding.PathToNearest(TilePosition, targets);
        return walkPath != null;
    }

    /// <summary>
    /// Similar to SetWalkTarget, but attempts to find a tile next to the target tile.
    /// </summary>
    public bool SetWalkTargetNextTo(Vector2Int target) {
        Vector2Int[] targets = {
            new Vector2Int(target.x + 1, target.y),
            new Vector2Int(target.x - 1, target.y),
            new Vector2Int(target.x, target.y + 1),
            new Vector2Int(target.x, target.y - 1)
        };
        return SetWalkTargetToNearest(targets);
    }

    public void CancelWalking() {
        walkPath = null;
    }
}

}