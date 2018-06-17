using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class LevelSelect : Node {
    private int selectedIndex = 0;
    private int cols = 4;
    private List<String> levels = new List<String> { "a", "b", "c", "d", "e", "f", "g", "h", "i" }; 
    private List<LevelSelectCard> cards = new List<LevelSelectCard>();

    public override void _Ready() {
        levels = GetActManager().acts;
        cards = CreateCards(levels, cols, 50, 50);
        var offest = new Vector2(45, 45);
        foreach (LevelSelectCard card in cards) {
            card.SetPosition(card.GetPosition() + offest);
            AddChild(card);
        }
        SelectIndex(0);
    }

    public override void _Process(float delta) {
        if (Input.IsActionJustPressed("ui_up")) {
            IncrementSelection(0, -1);
        }
        if (Input.IsActionJustPressed("ui_down")) {
            IncrementSelection(0, 1);
        }
        if (Input.IsActionJustPressed("ui_left")) {
            IncrementSelection(-1, 0);
        }
        if (Input.IsActionJustPressed("ui_right")) {
            IncrementSelection(1, 0);
        }
        if (Input.IsActionJustPressed("ui_accept")) {
            LoadSelectedLevel();
        }
    }

    public LevelSelect SelectIndex(int index) {
        selectedIndex = index % levels.Count;
        if (selectedIndex < 0) {
            selectedIndex = levels.Count - 1;
        }
        for (var i=0; i<cards.Count; i++) {
            cards[i].IsSelected = i == selectedIndex;
        }
        return this;
    }

    private ActManager GetActManager() => this.GetNode("/root/ActManager") as ActManager;

    private void LoadSelectedLevel() => GetActManager().GoToScene(levels[selectedIndex]);

    private void IncrementSelection(int x, int y) {
        var newSelection = indexToXy(selectedIndex, cols) + new Vector2(x, y);
        SelectIndex(xyToIndex((int)newSelection.x, (int)newSelection.y, cols));
    }

    private static Vector2 indexToXy(int i, int cols) =>
        new Vector2(i % cols, i / cols);

    private static int xyToIndex(int x, int y, int cols) =>
        y * cols + x;

    private static Vector2 CalculateCardPosition(int i, int cols, float horSpacing, float verSpacing) =>
        indexToXy(i, cols) * new Vector2(horSpacing, verSpacing);

    private static List<LevelSelectCard> CreateCards(List<String> levels, int cols, float horSpacing, float verSpacing) {
        var result = new List<LevelSelectCard>();
        for (var i = 0; i < levels.Count; i++) {
            var card = LevelSelectCard.Instantiate();
            card.Text = (i + 1).ToString();
            card.SetPosition(CalculateCardPosition(i, cols, horSpacing, verSpacing));
            card.IsSelected = false;
            result.Add(card);
        }
        return result;
    }
}
