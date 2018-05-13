using Godot;
using System;
using System.Collections.Generic;

public class CloneMenu : ReferenceRect {
    private struct ItemSlot {
        public Vector2 pos;
        public Vector2 size;
        public PlayerColor.Value color;
    }

    private List<CloneOptions.CloneOption> selections;
    private List<ItemSlot> items;
    private int selectedIndex = 0;
    private int cycle = 0;
    private bool isOpen = false;

    private Rect2 rect;
    private Vector2 inset;
    private Vector2 center;

    public void Next() {
        this.selectedIndex = (this.selectedIndex + 1) % this.selections.Count;
        this.Update();
    }

    public void Prev() {
        this.selectedIndex = ((this.selectedIndex - 1) + this.selections.Count) % this.selections.Count;
        this.Update();
    }

    public void SetOptions(List<CloneOptions.CloneOption> options) {
        this.selections = options;
    }

     public List<CloneOptions.CloneOption> GetOptions() {
        return this.selections;
    }

    public PlayerColor.Value GetSelectedColor() {
        return this.selections[selectedIndex].GetPlayerColor();
    }

    public override void _Ready() {
        // Init
        this.rect = this.GetRect();
        this.inset = new Vector2(1, 1);
        this.center = new Vector2(this.rect.Size.x / 2, this.rect.Size.y / 2);

        this.selectedIndex = 0;
        // TODO: Use this later if we get fancy with menus
        this.items = new List<ItemSlot>();
    }

    public int InputNext() {
        return Input.IsActionJustPressed("key_shoulder_right") ? 1 : 0;
    }

    public int InputPrev() {
        return Input.IsActionJustPressed("key_shoulder_left") ? 1 : 0;
    }

    public override void _Process(float delta) {
        int cycle = (this.InputNext() - this.InputPrev());
        if(cycle > 0) {
            this.Next();
        } else if(cycle < 0) {
            this.Prev();
        }
    }

    private void DrawOptions() {
        if (this.selections == null) {
            return;
        }

        CloneOptions.CloneOption c = this.selections[this.selectedIndex];

        // TODO: Draw available options

        // Draw current selection
        this.DrawRect(
            new Rect2(new Vector2((this.center.x - 8), this.center.y - 8), new Vector2(16, 16)),
            c.GetColor(),
            true
        );
    }

    public override void _Draw() {
        // Draw box (container)
        this.DrawRect(
            new Rect2(this.inset.x, this.inset.y, this.rect.Size.x - (this.inset.x * 2), this.rect.Size.y - (this.inset.y * 2)),
            new Color(256, 256, 256, 1), 
            true
        );

        // Draw inner box (sweet)
        this.DrawRect(
            new Rect2(this.inset.x * 2, this.inset.y * 2, this.rect.Size.x - (this.inset.x * 4), this.rect.Size.y - (this.inset.y * 4)),
            new Color(0, 0, 0, 1),
            true
        );

        this.DrawOptions();
    }
}
