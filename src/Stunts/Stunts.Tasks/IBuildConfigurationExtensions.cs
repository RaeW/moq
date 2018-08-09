namespace Stunts.Tasks
{
    static class IBuildConfigurationExtensions
    {
        public static ProjectReader GetProjectReader(this IBuildConfiguration configuration)
            => ProjectReader.GetProjectReader(configuration);

        public static SnapshotWorkspace GetWorkspace(this IBuildConfiguration configuration)
            => SnapshotWorkspace.GetWorkspace(configuration);
    }
}
