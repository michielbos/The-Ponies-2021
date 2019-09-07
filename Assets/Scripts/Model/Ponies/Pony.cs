using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using Controllers.Playmode;
using Controllers.Singletons;
using JetBrains.Annotations;
using Model.Actions;
using Model.Data;
using Scripts;
using Scripts.Proxies;
using UnityEngine;

namespace Model.Ponies {

public class Pony: MonoBehaviour, ITimeTickListener, IActionProvider {
    private const float WalkSpeed = 3f;
    
    public GameObject indicator;
    public Material indicatorMaterial;
    public List<PonyAction> queuedActions = new List<PonyAction>();
    [CanBeNull] public PonyAction currentAction;

    public Guid uuid;
    public string ponyName;
    public PonyRace race;
    public Gender gender;
    public PonyAge age;
    public Needs needs;

    [CanBeNull] private Path walkPath;
    private Vector2Int? nextWalkTile;

    public bool IsSelected => HouseholdController.Instance.selectedPony == this;
    public bool IsWalking => nextWalkTile != null || walkPath != null;
    public Vector2Int? WalkTarget => walkPath?.Destination;
    public bool WalkingFailed { get; private set;  }
    
    public Vector2Int TilePosition {
        get {
            Vector3 position = transform.position;
            return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        }
        set { transform.position = new Vector3(value.x, 0, value.y); }
    }

    public void InitInfo(Guid uuid, string ponyName, PonyRace race, Gender gender, PonyAge age) {
        this.uuid = uuid;
        this.ponyName = ponyName;
        this.race = race;
        this.gender = gender;
        this.age = age;
        indicator.GetComponent<Renderer>().material = new Material(indicatorMaterial);
    }

    public void InitGamePony(float x, float y, Needs needs, PonyActionData[] actionQueue) {
        this.needs = needs;
        transform.position = new Vector3(x, 0, y);
        queuedActions.AddRange(actionQueue.Select(data => ScriptPonyAction.FromData(this, data)));
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
                // Ponies shouldn't walk without an action.
                ClearWalkTarget();
                UpdateQueue();
            }
        }
    }

    public GamePonyData GetGamePonyData() {
        PonyActionData[] queueData = queuedActions.Select(action => action.GetData()).ToArray();
        return new GamePonyData(uuid.ToString(), transform.position.x, transform.position.z, needs.GetNeedsData(),
            queueData);
    }
    
    public PonyInfoData GetPonyInfoData() {
        return new PonyInfoData(uuid.ToString(), ponyName, (int) race, (int) gender, (int) age);
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
        return ScriptManager.Instance.hooks.RequestPonyActions(pony, this);
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
        WalkingFailed = walkPath == null;
        return !WalkingFailed;
    }
    
    public void ClearWalkTarget() {
        walkPath = null;
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