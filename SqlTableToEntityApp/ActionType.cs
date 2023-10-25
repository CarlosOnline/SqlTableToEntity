namespace SqlTableToEntityApp
{
    internal enum ActionType
    {
        /// <summary>
        /// Generate table entities.
        /// </summary>
        Entity,

        /// <summary>
        /// Generate database context.
        /// </summary>
        Context,

        /// <summary>
        /// Generate json files.
        /// </summary>
        Json,

        /// <summary>
        /// Validate template.
        /// </summary>
        Validate,
    }
}
