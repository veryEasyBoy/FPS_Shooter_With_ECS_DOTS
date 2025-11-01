using Zenject;

public class GlobalInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		Container.BindInterfacesAndSelfTo<PlayerInputActions>().AsSingle();
		Container.BindInterfacesAndSelfTo<DesktopInput>().AsSingle();
	}
}

