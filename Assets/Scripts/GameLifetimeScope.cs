using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RPGGame
{
    public class GameLifetimeScope : LifetimeScope
    {
        public static GameLifetimeScope Active;
        [Header("Scene References (Drag from hierarchy)")]
        [SerializeField] private GameEngine gameEngine;
        [SerializeField] private UIController uiController;
        private PartyInfo partyInfo;
        private Battle battle;
        private GameDataBase gameData;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(this.uiController);
            builder.RegisterComponent(this.gameEngine);
            this.partyInfo = new PartyInfo();
            builder.RegisterComponent(this.partyInfo);
            this.battle = new Battle();
            builder.RegisterComponent(this.battle);
            this.gameData = new GameDataBase();
            builder.RegisterComponent(this.gameData);

            builder.Register<Map>(Lifetime.Transient);
            builder.Register<Menu>(Lifetime.Transient);
            builder.Register<Effect>(Lifetime.Transient);
            builder.Register<Town>(Lifetime.Transient);
            builder.Register<Warrior>(Lifetime.Transient);
            builder.Register<Rouge>(Lifetime.Transient);
            builder.Register<Wizard>(Lifetime.Transient);
            builder.Register<Cleric>(Lifetime.Transient);
            builder.Register<Equipment>(Lifetime.Transient);
        }
    }
}