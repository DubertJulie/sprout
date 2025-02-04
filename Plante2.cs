	using Godot;
	using System;


	public partial class Plante2 : Node2D
	{

		[Export] public Texture2D plant0;
		[Export] public Texture2D plant1;
		[Export] public Texture2D plant2; 
		[Export] public Texture2D plant3;
		[Export] public Texture2D plantpara1;
		[Export] public Texture2D plantpara2; 
		[Export] public Texture2D plantpara3; 

		private Sprite2D _sprite;
		//private Button _tailler;
		private Button _arroser;
		private Button _deparasiter;
		private Button _fermer;
		// private Timer _growthTimer;
		private TextureProgressBar _waterBar;
		private TextureProgressBar _hpBar;
		private TextureProgressBar _growthBar;
		private Timer _timer;
		private double _hp; 
		private double max_hp;
		private int max_water;
		private double _croissance; 
		private int _water;
		private bool _parasites; 
		private bool _arrosee; 
		private bool _dead; 
		private bool _selected;
				
		public override void _Ready()
		{
			_sprite = GetNode<Sprite2D>("Sprite2D");
			max_hp = 10;
			_hp = 5; // on démarre midlife
			_croissance = 1; // on démarre au premier stade de croissance
			max_water = 10;
			_water = 5; 
			_parasites = false;
			_selected = false;
			
			GetNode<Control>("UI_Panel").Visible = false; 

			_arroser = GetNode<Button>("UI_Panel/ButtonArroser");
			_deparasiter = GetNode<Button>("UI_Panel/ButtonDeparasiter");
			_fermer = GetNode<Button>("UI_Panel/ButtonFermer");
			
			_arroser.Connect("pressed", new Callable(this, nameof(ArroserPlante)));
			_deparasiter.Connect("pressed", new Callable(this, nameof(DeparasiterPlante)));
			_fermer.Connect("pressed", new Callable(this, nameof(FermerUI)));
			
			GetNode<Control>("ProgressBar").Visible = true; 

			_waterBar = GetNode<TextureProgressBar>("ProgressBar/ProgressBarWater");
			_waterBar.MaxValue = 10; 
			_waterBar.Value = _water;
			
			_hpBar = GetNode<TextureProgressBar>("ProgressBar/ProgressBarHp");
			_hpBar.MaxValue = 10;
			_hpBar.Value = _hp;
			
			_growthBar = GetNode<TextureProgressBar>("ProgressBar/ProgressBarGrowth");
			_growthBar.MaxValue = 5;
			_growthBar.Value = _croissance;
			
			GetNode<Area2D>("Area2D").Connect("input_event", new Callable(this, nameof(OnPlanteClick)));
						
			_timer = GetNode<Timer>("Timer");
			_timer.Timeout += OnTimerTimeout;
			
			_timer.Start();
		}
			
		private void OnPlanteClick(Node viewport, InputEvent @event, int shape_idx)
		{
			// Il faut cliquer sur la plante pour accéder à ses options
			if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{				
				
				if (!_selected) {
					GD.Print("sélectionné");
				  	GetNode<Control>("UI_Panel").Visible = true; 
					_selected = true;
				} else {
					_selected = false;
				}
			}
		}
		
		private void FermerUI() {
			GetNode<Control>("UI_Panel").Visible = false;
			_selected = false;
		}
		
		private void OnTimerTimeout() {
			//GD.Print("Hp = " + _hp + "/10");
			//GD.Print("Eau = " + _water + "/10");
			//GD.Print("Croissance = " + _croissance + "/5");
			//GD.Print("Parasites = " + _parasites);
			_arrosee = false;
			

			
			_water -= 1;
			if (_water >= max_water) {
				_water = max_water;
			}
			_waterBar.Value = _water;
					
			var random = new RandomNumberGenerator();
			random.Randomize();
			bool parasited = false;
			
			if (_water < 4) {
				parasited = random.RandiRange(0, 3) == 1;	
			} else 
			{
				parasited = random.RandiRange(0, 6) == 1;	
			}
			
			if (parasited) {
				_parasites = true; 
			}
					
			if (_water >= 5 && !_parasites) {
				_hp += 0.5;
				_croissance += 0.50;
				_growthBar.Value = _croissance;
			} 
			else if (_water > 0 && _water < 5) {
				_hp -= _parasites ? 2 : 1; 
			} 
			else if (_water <= 0) {
				_hp -= _parasites ? 4 : 2; 
			}

			if (_hp <= 0) {
				_croissance = 0; 
				_growthBar.Value = _croissance;
				GD.Print("Finito");
				_dead = true;
				GetNode<Control>("ProgressBar").Visible = false; 
				_timer.Stop();
			}
			
			if (_hp >= max_hp) {
				_hp = max_hp;
			}
			_hpBar.Value = _hp;
			GrandirPlante();
		}
		
		private void GrandirPlante() {
			int croissance = (int)Math.Round(_croissance); 
			switch(croissance) {
				case 0 :
					_sprite.Texture = plant0;
					break;
				case 1 :
					if (_parasites) {
						_sprite.Texture = plantpara1;
					} else {
					_sprite.Texture = plant1;
					}
					break;
				case 2 :
					if (_parasites) {
						_sprite.Texture = plantpara2;
					} else {
					_sprite.Texture = plant2;}
					break;
				case 3 :
					if (_parasites) {
						_sprite.Texture = plantpara3;
					} else {
						_sprite.Texture = plant3;
					}
					break;
			}
		}
		
		private void ArroserPlante()
		{
			if (_arrosee == false) {
				_water += 4; 
				if (_water >= max_water) {
				_water = max_water;
				}
				_waterBar.Value = _water;
				GD.Print("La plante a été arrosée !");
				_arrosee = true;
			} else {
				GD.Print("La plante a déjà été arrosée");
			}
		}
		
		private void DeparasiterPlante()
		{
			if (_parasites == true) {
				_parasites = false; 
				_hp += 1;
				_hpBar.Value = _hp;
				GrandirPlante();
			}
		}
			
				
	}
