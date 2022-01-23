using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveCubo : MonoBehaviour {
	
	public Transform cubo;
	public Camera cam;
	
	public Rect posicao;
	private string script = "";
	public Vector2 scrollscript = new Vector2(50,50);
	
	private int length;
	private Transform[] tf = new Transform[10];
	private Vector3 eixo, centro;
	private float rotacao, razao;

	private int mudarFace, baseCubo;

	private int misturar, old;
	
	public int[][] m;
	
	public Material [] mat;
	public Transform[] faces;
	
	private List<Vector3> PosDefault = new List<Vector3>();
	private List<Quaternion> RotDefault = new List<Quaternion>();
	
	private List<int> Algoritmo = new List<int>();
	
	private Buffer buffer = new Buffer();

	//\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\//
	// rotina para rotacao da matriz de cores
	private void moveCor( string parametro ){
    	int i,j,k;
		int[] p = new int[14];
    	
	    //Decodifica parametros
	    for(i=0,j=0; i<parametro.Length; i++){
	      if( parametro[i] == '-' )
	        p[j++] = (int)(parametro[++i]-0x30)*(-1);
	      else if( parametro[i] >= '0' && parametro[i] <= '9' )
	        p[j++] = (int)parametro[i] - 0x30;
	    }
	    
	    if( p[1] != 1 ) k = 1;    // sentido anti-horario
	    else k = 3;               // sentido horario
	    
	    for(; k>0; k-- ){
	       int[] temp= { m[p[0]][6], m[p[0]][3], 0 };
			
	       m[p[0]][6] = m[p[0]][0];
	       m[p[0]][3] = m[p[0]][1];
	       m[p[0]][0] = m[p[0]][2];
	       m[p[0]][1] = m[p[0]][5];
	       m[p[0]][2] = m[p[0]][8];
	       m[p[0]][5] = m[p[0]][7];
	       m[p[0]][8] = temp[0];
	       m[p[0]][7] = temp[1];
	       
	       temp[0] = m[ p[2] ][ p[3]+(0*p[4]) ];
	       temp[1] = m[ p[2] ][ p[3]+(1*p[4]) ];
	       temp[2] = m[ p[2] ][ p[3]+(2*p[4]) ];
	       
	       for( i=0; i<3; i++){
	          j = (i*3)+2;
	          m[ p[j] ][ p[j+1]+(0*p[j+2]) ] = m[ p[j+3] ][ p[j+4]+(0*p[j+5]) ];
	          m[ p[j] ][ p[j+1]+(1*p[j+2]) ] = m[ p[j+3] ][ p[j+4]+(1*p[j+5]) ];
	          m[ p[j] ][ p[j+1]+(2*p[j+2]) ] = m[ p[j+3] ][ p[j+4]+(2*p[j+5]) ];
	       }
	       
	       j = (i*3)+2;
	       m[ p[j] ][ p[j+1]+(0*p[j+2]) ] = temp[0];
	       m[ p[j] ][ p[j+1]+(1*p[j+2]) ] = temp[1];
	       m[ p[j] ][ p[j+1]+(2*p[j+2]) ] = temp[2];       
	    }
	}
	
	//\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\//
	// rotina para movimento das faces do cubo
	public bool Move( int face ){
		int i,j;
		string parametroCor = "";
		
		int sentido;
		
		if( (face & 8) > 0 ) sentido = 1;
		else sentido = 0;

		// se ainda estiver rotacionando
		if( rotacao > 0 ) return false;

		// se rotacao de face
		if( face < 0x10 ){
			face &= 7;
			sentido++;

			// procura das pecas a mover
			for(i=0,j=0; i<length; i++){
				tf[j] = cubo.GetChild(i);
				
				// Rotacao Direita (azul)  Anti-Horario
				if( face == 0 ){
					if( i==0 ) sentido--;	// inverte para Horario
					
					eixo = new Vector3(0,0,1);
					if( tf[j].transform.position.z < -9.9 ) j++;
					parametroCor = "0"+sentido.ToString()+"52+336-342+312+3"; // 12+342+336-352+3";
				}
				
				// Rotacao Frente (Laranja)  Anti-Horario
				else if( face == 1 ){
					if( i==0 ) sentido--;	// inverte para Horario
					
					eixo = new Vector3(1,0,0); 
					if( tf[j].transform.position.x < -1.9 ) j++;
					parametroCor = "1"+sentido.ToString()+"42-128-356+100+3"; // 00+356+128-342-1";
				}
				
				// Rotacao Esquerda (Verde)  Horario - Invertido
				else if( face == 2 ){
					eixo = new Vector3(0,0,1);
					if( tf[j].transform.position.z > -6.1 ) j++;
					parametroCor = "2"+(sentido-1).ToString()+"40+338-350+310+3"; // 56-332+346-316-3";
				}
				
				// Rotacao Fundo (Vermelho)   Horario - Invertido
				else if( face == 3 ){
					eixo = new Vector3(1,0,0);
					if( tf[j].transform.position.x > 1.9 ) j++;
					parametroCor = "3"+(sentido-1).ToString()+"50+126-348-102+3";
				}
				
				// Rotacao do Cima (branco)   Horario - Invertido
				else if( face == 4 ){
					eixo = new Vector3(0,1,0);
					if( tf[j].transform.position.y > 1.9 ) j++; 
					parametroCor = "4"+(sentido-1).ToString()+"06+136+126+116+1";
				}
				
				// Rotacao Baixo (amarelo)   Anti-Horario
				else if( face == 5 ){
					if( i==0 ) sentido--;	// inverte para Horario
					
					eixo = new Vector3(0,1,0);
					if( tf[j].transform.position.y < -1.9 ) j++;
					parametroCor = "5"+sentido.ToString()+"10+120+130+100+1"; // 00+130+120+110+1";
				}
				
			}
			// se face nao existe
			if( j == 0 ) return false;
		}

		// se mudanca de face
		else if( face == 0x20 ){
			eixo = new Vector3(0,0,1);
			sentido = 0;
			mudarFace = 1;
		}
		else if( (face & 0x10) == 0x10 ){
			sentido = face & 0x01;
			eixo = new Vector3(0,1,0);
			mudarFace = 1;
		}
		
		rotacao=10;						// velocidade de rotacao
		razao=90/rotacao;				// controlador de rotacao
		if( sentido != 1 )razao *= -1;  // ajusta sentido	Horario
		moveCor( parametroCor );
		
		return true;
	}
	
	void carregaAlgoritmo(){
		//try{
		{
			string[] param;
			char[] separador = {' ', ',', ';', (char)10 , (char)13 };
			param = script.Split(separador, System.StringSplitOptions.RemoveEmptyEntries);
			
			int temp;
			
			// se carregar matriz
			if( string.Compare( param[0], "load", true ) == 0 ){
				int i,j,k;

				if( param.Length == 7 || param.Length == 8 ){
					for( i=0,k=1; i<6; i++ ){
						for( j=0; j<9; j++,k++ ){
							m[i][j] = (int) (param[i+1][j]-0x30);
							faces[k-1].GetComponent<Renderer>().material = mat[m[i][j]];
						}
					}
				}
				else{
					for( i=0,k=1; i<6; i++ ){
						for( j=0; j<9; j++,k++ ){
							m[i][j] = (int) (param[k][0]-0x30);
							faces[k-1].GetComponent<Renderer>().material = mat[m[i][j]];
						}
					}
				}
			}
			
			else if( param[0] == "teste" ){
				for( int i=0; i<transform.childCount; i++){
					transform.GetChild(i).position = PosDefault[i];
					transform.GetChild(i).rotation = RotDefault[i];
				}
			}
			
			else {
				foreach( string i in param ){				
					for(int j=0; j<i.Length; j++){
						temp = 0;
					
						if( string.Compare(i, "R",true)==0 || string.Compare(i, "R\'",true)==0 )temp = 1;
						else if( i[j]=='f' || i[j]=='F' )temp = 2;
						else if( i[j]=='l' || i[j]=='L' )temp = 3;
						else if( string.Compare(i, "B",true)==0 || string.Compare(i, "B\'",true)==0 )temp = 4;
						else if( i[j]=='u' || i[j]=='U' )temp = 5;
						else if( i[j]=='d' || i[j]=='D' )temp = 6;
						else if( i[j]=='m' || i[j]=='M' ){
							if( j+1 < i.Length ){
								temp = i[j+1] - 0x2F;
								j++;
							}
						}
						else if( i[j] == '2' ){
							if( j+2 < i.Length && (i[j+1]=='m'||i[j+1]=='M') ){
								temp = i[j+2] - 0x2F;
								j+=2;
							}
						}
						// comando de mudanca de face
						else if( i[j]=='c' || i[j]=='C' ){
							if( j+1 < i.Length ) temp = i[++j] - 0x30;
							else temp = 1;

							for( int loop=0; loop<temp; loop++)
								Algoritmo.Add(0x20);
							temp = 0;
						}
						// Comando de mudar base
						else if( i[j]=='b' || i[j]=='B' ){
							if( j+1 < i.Length ) temp = i[++j] - 0x30;

							if( baseCubo == 0 ){
								for( int loop=0; loop<temp; loop++ )
									Algoritmo.Add(0x10);
							}
							else if( baseCubo == 1 ){
								if( temp == 2 ) Algoritmo.Add(0x10);
								else if( temp == 0 ) Algoritmo.Add(0x11);
							}
							else if( baseCubo == 2 ){
								for( int loop=2; loop>temp; loop-- )
									Algoritmo.Add(0x11);
							}
							baseCubo = temp;
							temp = 0;
						}
						// se comando de rotacionar face
						else if( i[j]=='r' || i[j]=='R' ){
							if( j+1 < i.Length ) temp = i[++j] - 0x30;
							
							if( baseCubo == 0 ){
								for( int loop=0; loop<temp; loop++ )
									Algoritmo.Add(5);
							}
							else if( baseCubo == 1 ){
								if( temp == 2 ) Algoritmo.Add(5);
								else if( temp == 0 ) Algoritmo.Add(8+5);
							}
							else if( baseCubo == 2 ){
								for( int loop=2; loop>temp; loop-- )
									Algoritmo.Add(8+5);
							}
							baseCubo = temp;
							temp = 0;
						}

						
						if( temp > 0 ){
							if( (j+1)<i.Length && i[j+1] == '\'' ){
								temp += 8;
								j++;
							}
							if( 2 < i.Length && i[0]=='2' && (i[1]=='m'||i[1]=='M')){
								Algoritmo.Add( temp-1 );
							}
							Algoritmo.Add( temp-1 );
						}
					}
				}
			}
		}
		//} 
		//catch{ }
	}
	
	void moveCamera(){
		cam.transform.RotateAround( cubo.transform.position, new Vector3(0,1,0), 1 );
	}
	
	// Use this for initialization
	void Start () {		
		
		// Inicializa matriz de cores
		m = new int[6][];
		for( int i=0; i<6; i++ ){
			m[i] = new int[9];
			for( int j=0; j<9; j++)
				m[i][j] = i;
		}
		
		for( int i=0; i<transform.childCount; i++ ){
			PosDefault.Add( transform.GetChild(i).position );
			RotDefault.Add( transform.GetChild(i).rotation );
		}

		mudarFace = 0;
		baseCubo = 1;
	}
/*
	void OnLevelWasLoaded(){
		buffer = GameObject.Find("Buffer").GetComponent<Buffer>();

		if( !buffer.isEmpty() ){
			script = buffer.getBuffer();
			carregaAlgoritmo();
		}
	}
*/
	// On GUI
	void OnGUI(){
		
		float tx, ty;
		
		tx = Screen.width/2; ty = Screen.height;
		
		GUI.BeginGroup( new Rect(tx+20, 40, tx-30, ty-50 ) );
        scrollscript = GUILayout.BeginScrollView(scrollscript, GUILayout.Width(tx-40), GUILayout.Height(ty-50));
        script = GUILayout.TextArea( script );
        GUILayout.EndScrollView();
      	GUI.EndGroup();
		
		GUI.Label(new Rect( 15, ty-20, 200, 40 ), "Por: Renan Pacheco");
		GUI.Label(new Rect( 15, 10, 200, 40 ), "Cubo Magico");
		
		
		// Botao para Misturar Cubo
		if( GUI.Button( new Rect(tx+10,5,60,30) , "Misturar") && misturar < 1){
			misturar = 25;
		}
		
		// Botao para retorna a matriz de cores
		if( GUI.Button( new Rect(tx+80,5,40,30), "Get") ){
			script = "";
			for(int i=0; i<6; i++){
				for(int j=0; j<9; j++)
					script += m[i][j].ToString() + " ";
				script += "\n";
			}
		}
		
		// Botao para Executar script
		if( GUI.Button( new Rect(tx+130,5,50,30), "Run" )){
			carregaAlgoritmo();
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
		length = cubo.childCount;
		centro = cubo.GetChild(13).transform.position;
		centro += cubo.transform.position;
		
		////////////////////////////////////////
		// Rotina de rotacao sucessiva do cubo
		if( rotacao > 0 ){
			int i;

			// se mudar face
			if( mudarFace == 1 ){
				for(i=0; i<length; i++)
					cubo.GetChild(i).RotateAround( centro, eixo , razao);
			}
			// se movimento de face
			else {
				for(i=0; i<9; i++)
					tf[i].RotateAround( centro, eixo , razao);
			}
			
			rotacao--;
		}
		
		////////////////////////////////////////
		// Rotina de embaralhar
		else if( misturar > 0 ){
			int f, s;
			
			f = Random.Range(0, 6);
			s = Random.Range(0, 2);
			
			if( f!=old && Move( f + (s*8) ) ){
				misturar--;
				old = f;
			}
		}
		
		else if( rotacao<1 && Algoritmo.Count>0){
			int temp = Algoritmo[0];
			Algoritmo.RemoveAt(0);
			if( mudarFace == 1 ) mudarFace = 0;
			Move( temp );
		}
		
		////////////////////////////////////
		// verifica acoes do usuario
		if( Input.GetKeyUp(KeyCode.Alpha1) ){
			if( Input.GetKey(KeyCode.LeftShift) )
				Move(0 + 8);
			else 
				Move(0);
		}
		else if( Input.GetKeyUp(KeyCode.Alpha2) ){
			if( Input.GetKey(KeyCode.LeftShift) )
				Move(1 + 8);
			else 
				Move(1);
		}
		else if( Input.GetKeyUp(KeyCode.Alpha3) ){
			if( Input.GetKey(KeyCode.LeftShift) )
				Move(2 + 8);
			else 
				Move(2);
		}
		else if( Input.GetKeyUp(KeyCode.Alpha4) ){
			if( Input.GetKey(KeyCode.LeftShift) )
				Move(3 + 8);
			else 
				Move(3);
		}
		else if( Input.GetKeyUp(KeyCode.Alpha5) ){
			if( Input.GetKey(KeyCode.LeftShift) )
				Move(4 + 8);
			else 
				Move(4);
		}
		else if( Input.GetKeyUp(KeyCode.Alpha6) ){
			if( Input.GetKey(KeyCode.LeftShift) )
				Move(5 + 8);
			else 
				Move(5);
		}
	}
}
