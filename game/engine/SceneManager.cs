using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class SceneManager {
    private List<Entity> entities;
    private System.Collections.Generic.Dictionary<Type, List<Entity>> entitiesByType;

    public SceneManager() {
        this.entities = new List<Entity>();
        this.entitiesByType = new System.Collections.Generic.Dictionary<Type, List<Entity>>();
    }

    public void Add(Entity entity) {
        this.entities.Add(entity);
        if (!this.entitiesByType.ContainsKey(entity.GetType())) {
            this.entitiesByType.Add(entity.GetType(), new List<Entity>());
        }
        this.entitiesByType[key: entity.GetType()].Add(entity);
        GD.Print("Added: ", entity.GetName());
    }

    public void Remove(Entity entity) {
        this.entities.Remove(entity);
        this.entitiesByType[key: entity.GetType()].Remove(entity);
    }

    public List<Entity> GetEntities() => this.entities;
    public List<Entity> GetEntitiesByType(Type t) => this.entitiesByType[key: t];
    public List<T> GetEntitiesBy<T>() where T : Entity {
        if(!this.entitiesByType.ContainsKey(typeof(T))) {
            return new List<T>();
        }
        return this.entitiesByType[key: typeof(T)].Cast<T>().ToList();
    }
    public List<T> FindEntitiesBy<T>() where T : Entity =>
        this.entities.Where(e => e.GetType() == typeof(T) || e.GetType().IsSubclassOf(typeof(T))).Cast<T>().ToList();
}
