namespace PloppableRICO
{
    /// <summary>
    /// A simple UI panel to show any mod conflict notifications.
    /// </summary>
    class ConflictNotification : ErrorNotification
    {
        /// <summary>
        /// Creates the panel object in-game.
        /// </summary>
        public override void Create()
        {
            base.Create();
            instance = uiGameObject.AddComponent<ConflictNotification>();
        }


        /// <summary>
        /// Sets up the panel; called by Unity just before any of the Update methods is called for the first time..
        /// </summary>
        public override void Start()
        {
            headerText = "Mod conflict detected";

            base.Start();
        }
    }
}