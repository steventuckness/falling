using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class CollisionEngine {
    #region Set_Up
    private Entity entity;
    private Scene scene;

    public CollisionEngine(Entity entity, Scene scene) {
        this.entity = entity;
        this.scene = scene;
    }
    #endregion Set_Up

    public bool CollideCheck<Against>(Vector2 position) where Against : Entity {
        Vector2 pos = this.entity.GetPosition();
        this.entity.SetPosition(position);
        Collider c = this.entity.GetCollider();
        List<Against> entities = this.scene.GetManager().FindEntitiesBy<Against>();
        bool hit = false;
        foreach (Against e in entities) {
            Collider otherCollider = e.GetCollider();
            if (e != this.entity && otherCollider.IsCollidable && c.Collides(otherCollider)) {
                hit = true;
                break;
            }
        }
        this.entity.SetPosition(pos);
        return hit;
    }

    public Against CollideFirst<Against>(Vector2 position) where Against : Entity {
        Against firstHit = null;
        Vector2 pos = this.entity.GetPosition();
        this.entity.SetPosition(position);
        Collider c = this.entity.GetCollider();
        List<Against> entities = this.scene.GetManager().FindEntitiesBy<Against>();
        foreach (Against e in entities) {
            Collider ec = e.GetCollider();
            if (e != this.entity && c.Collides(ec)) {
                firstHit = e;
                break;
            }
        }
        this.entity.SetPosition(pos);
        return firstHit;
    }

    public List<Against> CollideAll<Against>(Vector2 position) where Against : Entity {
        List<Against> hits = new List<Against>();
        Vector2 pos = this.entity.GetPosition();
        this.entity.SetPosition(position);
        Collider c = this.entity.GetCollider();
        List<Against> entities = this.scene.GetManager().FindEntitiesBy<Against>();
        foreach (Against e in entities) {
            Collider ec = e.GetCollider();
            if (e != this.entity && c.Collides(ec)) {
                hits.Add(e);
            }
        }
        this.entity.SetPosition(pos);
        return hits;
    }
}
