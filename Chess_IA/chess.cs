using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Linq;

namespace Chess_IA
{
    class Par {
        public Par(int value1, int value2) {
            X = value1;
            Y = value2;
        }
        public Par(Par a){
            X = a.X;
            Y = a.Y;
        }

        public static bool operator==(Par a, Par b){
            return b.X==a.X && b.Y == a.Y;
        }

        public static bool operator !=(Par a, Par b)
        {
            return !(a == b);
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    class chess
    {
        public string[][] tab;
        public string[][] Qtab;
        public bool juega;
        public string leng_alg;
        public int jugada; 
        public string pmov;
        public Par inicio, fin;
        public Par ini_, fin_;
        public bool bkcheck, wkcheck;
        public bool bkmov, wkmov;
        public bool bt1mov, wt1mov;
        public bool bt2mov, wt2mov;

        public chess(){
            tab=new string[8][];
            Qtab = new string[8][];
            for (int i = 0; i < 8; i++)
            {
                tab[i] = new string[8];
                Qtab[i] = new string[8];
            }

            for(int i=0; i<8; i++)
                for(int j=0; j<8; j++)
                    tab[i][j] = "null";

            for(int j=0; j<8; j++){
                tab[1][j]="bp";
                tab[6][j]="wp";
            }

            tab[0][0]=tab[0][7]="br";
            tab[0][1]=tab[0][6]="bn";
            tab[0][2]=tab[0][5]="bb";
            tab[0][3]="bq";
            tab[0][4]="bk";

            tab[7][0]=tab[7][7]="wr";
            tab[7][1]=tab[7][6]="wn";
            tab[7][2]=tab[7][5]="wb";
            tab[7][3]="wq";
            tab[7][4]="wk";

            juega = false;
            jugada = 0;
            inicio = fin = new Par(-1, -1);
            ini_ = fin_ = new Par(-1, -1);
            bkcheck=wkcheck=false;
            bkmov=wkmov=false;
            bt1mov=wt1mov=false;
            bt2mov=wt2mov=false;
            leng_alg="";
        }


        public string this[int x, int y]
        {
            get { return tab[x][y]; }
            set { tab[x][y] = value; }
        }

        public chess(chess a){
            tab = new string[8][];
            for(int i=0; i<8; i++)
                tab[i]=new string[8];

            for(int i=0; i<8; i++)
                for(int j=0; j<8; j++)
                    tab[i][j]=a.tab[i][j];

            juega=a.juega;
            pmov = a.pmov;
            inicio = a.inicio;
            fin = a.fin;
            ini_ = a.ini_;
            fin_ = a.fin_;
            bkcheck=a.bkcheck;
            wkcheck=a.wkcheck;
            bkmov=a.bkmov;
            wkmov=a.wkmov;
            jugada = 0;
            bt1mov=a.bt1mov;
            wt1mov=a.wt1mov;
            bt2mov=a.bt2mov;
            wt2mov=a.wt2mov;
            leng_alg=a.leng_alg;
        }

        public void deshacer(chess a)
        {
            actualizar(a);
        }

        public void reset()
        {
            for(int i=0; i<8; i++)
                for(int j=0; j<8; j++)
                    tab[i][j] = "null";

            for(int j=0; j<8; j++){
                tab[1][j]="bp";
                tab[6][j]="wp";
            }

            tab[0][0]=tab[0][7]="br";
            tab[0][1]=tab[0][6]="bn";
            tab[0][2]=tab[0][5]="bb";
            tab[0][3]="bq";
            tab[0][4]="bk";

            tab[7][0]=tab[7][7]="wr";
            tab[7][1]=tab[7][6]="wn";
            tab[7][2]=tab[7][5]="wb";
            tab[7][3]="wq";
            tab[7][4]="wk";

            juega=false;
            inicio = fin = new Par(-1, -1);
            jugada = 0;
            bkcheck=wkcheck=false;
            bkmov=wkmov=false;
            bt1mov=wt1mov=false;
            bt2mov=wt2mov=false;
            leng_alg="";
        }

        public List<Par> movimientos(int i, int j)
        {
            List<Par> lista = new List<Par>();
            string p = tab[i][j];

            igual();
            if(p[1]=='p') lista = Qmov_peon(i, j);
            else if(p[1]=='r') lista = Qmov_torre(i, j);
            else if(p[1]=='n') lista = Qmov_caballo(i, j);
            else if(p[1]=='b') lista = Qmov_alfil(i, j);
            else if(p[1]=='k') lista = Qmov_rey(i, j);
            else if(p[1]=='q') {
                lista = Qmov_torre(i, j);
                lista.AddRange(Qmov_alfil(i, j));
            }
            return lista;
        }

        public bool hay_mov()
        {
            char c;
            if(!juega)
                c='w';
            else c='b';
            igual();
            for(int i=0; i<8; i++)
                for(int j=0; j<8; j++)
                    if(Qtab[i][j][0] == c){
                        List<Par> k = new List<Par>();
                        if (i_can_play(i, j, ref k))
                            return true;
                    }
            return false;
        }

        public bool jaque(string a)
        {
            Par p = get_pos(a);
            List<Par> l;
            string c;
            if(a[0]=='w')
                c = "b";
            else c = "w";

            l = Qmov_caballo(p.X, p.Y);
            foreach(Par p1 in l)
                if(Qtab[p1.X][p1.Y] == c+"n")
                    return true;
            l = Qmov_alfil(p.X, p.Y);
            foreach(Par p1 in l)
                if(Qtab[p1.X][p1.Y] == c+"b" || Qtab[p1.X][p1.Y] == c+"q")
                    return true;
            l = Qmov_torre(p.X, p.Y);
            foreach(Par p1 in l)
                if(Qtab[p1.X][p1.Y] == c+"r" || Qtab[p1.X][p1.Y] == c+"q")
                    return true;
            l = Qmov_peon_p(p.X, p.Y);
            foreach(Par p1 in l)
                if(Qtab[p1.X][p1.Y] == c+"p")
                    return true;
            l = Qmov_rey_p(p.X, p.Y);
            foreach(Par p1 in l)
                if(Qtab[p1.X][p1.Y] == c+"k")
                    return true;
            return false;
        }

        public void Mover(Par mv, Par p)
        {
            if(Qtab[p.X][p.Y][1] == 'p' && p.Y != mv.Y && Qtab[mv.X][mv.Y] == "null"){
                Qtab[p.X][mv.Y] = "null";
            }
            Qtab[mv.X][mv.Y] = Qtab[p.X][p.Y];
            Qtab[p.X][p.Y]="null";
        }

        public void igual()
        {
            for(int i =0; i<8; i++){
                for(int j =0; j<8; j++){
                    Qtab[i][j] = tab[i][j];
                }
            }
        }

        int contain(List<Par> L, Par p)
        {
            int s = L.Count();
            for (int i = 0; i < s; i++)
                if (L[i] == p)
                    return i;
            return -1;
        }

        public void act_lista_jaque(ref List<Par> l, Par p)
        {
            int t=0, size = l.Count();
            if (size > 0)
                for (int i = 0; i < size; i++)
                {
                    igual();
                    Mover(l[i], p);
                    if (!juega)
                    {
                        if (jaque("wk"))
                        {
                            l.RemoveAt(i);
                            i--;
                            size--;
                        }
                        else t++;
                    }
                    else
                    {
                        if (jaque("bk"))
                        {
                            l.RemoveAt(i);
                            i--;
                            size--;
                        }
                        else t++;
                    }
                }
            if (size > 0)
            {
                if (!juega)
                {
                    if (!wkmov)
                    {
                        int tt = contain(l, new Par(p.X, p.Y-1));
                        if (tt == -1)
                        {
                            tt = contain(l, new Par(p.X, p.Y - 2));
                            if (tt != -1)
                            {
                                l.RemoveAt(tt);
                                size --;
                            }
                        }
                        tt = contain(l, new Par(p.X, p.Y + 1));
                        if (tt == -1 && size > 0)
                        {
                            tt = contain(l, new Par(p.X, p.Y + 2));
                            if (tt != -1)
                            {
                                l.RemoveAt(tt);
                                size--;
                            }
                        }
                    }
                }
                else
                {
                    if (!bkmov)
                    {
                        int tt = contain(l, new Par(p.X, p.Y - 1));
                        if (tt == -1)
                        {
                            tt = contain(l, new Par(p.X, p.Y - 2));
                            if (tt != -1)
                            {
                                l.RemoveAt(tt);
                                size--;
                            }
                        }
                        tt = contain(l, new Par(p.X, p.Y + 1));
                        if (tt == -1 && size > 0)
                        {
                            tt = contain(l, new Par(p.X, p.Y + 2));
                            if (tt != -1)
                            {
                                l.RemoveAt(tt);
                                size--;
                            }
                        }
                    }
                }
            }
        }

        public Par get_pos(string a)
        {
            for(int i =0; i<8; i++)
                for(int j =0; j<8; j++)
                    if(Qtab[i][j]==a){
                        return new Par(i,j);
                    }
            return new Par(-1,-1);
        }

        public void actualizar(chess a)
        {
            for(int i=0; i<8; i++)
                for(int j=0; j<8; j++)
                    tab[i][j]=a.tab[i][j];
            juega=a.juega;
            pmov = a.pmov;
            inicio = a.inicio;
            fin = a.fin;
            ini_ = a.ini_;
            fin_ = a.fin_;
            bkcheck=a.bkcheck;
            wkcheck=a.wkcheck;
            bkmov=a.bkmov;
            wkmov=a.wkmov;
            bt1mov=a.bt1mov;
            jugada = a.jugada;
            wt1mov=a.wt1mov;
            bt2mov=a.bt2mov;
            wt2mov=a.wt2mov;
            leng_alg=a.leng_alg;
        }
        public void set_tab(int i, int j, string p)
        { 
            tab[i][j]=p;
        }

        public string get_tab(int i, int j)
        {
            return tab[i][j];
        }

        public bool i_can_play(int i, int j, ref List<Par> L)
        {
            if(!juega){
                if(tab[i][j][0] == 'w'){
                    L = movimientos(i, j);
                    act_lista_jaque(ref L, new Par(i, j));

                    if(L.Count() != 0)
                        return true;
                }
            }
            else {
                if(tab[i][j][0] == 'b'){
                    L = movimientos(i, j);
                    act_lista_jaque(ref L, new Par(i, j));

                    if(L.Count() != 0)
                        return true;
                }
            }
            return false;
        }

        public List<Par> Qmov_peon(int i, int j)
        {
            List<Par> lista = new List<Par>();
            if(!juega){
                if(Qtab[i-1][j]=="null")
                    lista.Add(new Par(i-1, j));
                if(i==6 && Qtab[i-2][j]=="null" && Qtab[i-1][j]=="null")
                    lista.Add(new Par(i-2, j));

                if(j==0 && Qtab[i-1][j+1][0] == 'b')
                    lista.Add(new Par(i-1, j+1));
                else if(j == 7 && Qtab[i-1][j-1][0] == 'b')
                    lista.Add(new Par(i-1, j-1));
                else if(j>0 && j<7){
                    if(Qtab[i-1][j+1][0] == 'b')
                        lista.Add(new Par(i-1, j+1));
                    if(Qtab[i-1][j-1][0] == 'b')
                        lista.Add(new Par(i-1, j-1));
                }
                if(pmov == "bp" && i == 3){
                    if(inicio.X==1 && fin.X==3){
                        if(j==0 && fin.Y==j+1)
                            lista.Add(new Par(i-1, j+1));
                        else if(j==7 && fin.Y==j-1)
                            lista.Add(new Par(i-1, j-1));
                        else if(j>0 && j<7 && fin.Y == j+1)
                            lista.Add(new Par(i-1, j+1));
                        else if(j>0 && j<7 && fin.Y == j-1)
                            lista.Add(new Par(i-1, j-1));
                    }
                }
            }
            else{
                if(Qtab[i+1][j]=="null")
                    lista.Add(new Par(i+1, j));
                if(i==1 && Qtab[i+2][j]=="null" && Qtab[i+1][j]=="null")
                    lista.Add(new Par(i+2, j));

                if(j==0 && Qtab[i+1][j+1][0] == 'w')
                    lista.Add(new Par(i+1, j+1));
                else if(j == 7 && Qtab[i+1][j-1][0] == 'w')
                    lista.Add(new Par(i+1, j-1));
                else if(j>0 && j<7){
                    if(Qtab[i+1][j+1][0] == 'w')
                        lista.Add(new Par(i+1, j+1));
                    if(Qtab[i+1][j-1][0] == 'w')
                        lista.Add(new Par(i+1, j-1));
                }
                if(pmov == "wp" && i == 4){
                    if(inicio.X==6 && fin.X==4){
                        if(j==0 && fin.Y==j+1)
                            lista.Add(new Par(i+1, j+1));
                        else if(j==7 && fin.Y==j-1)
                            lista.Add(new Par(i+1, j-1));
                        else if(j>0 && j<7 && fin.Y == j+1)
                            lista.Add(new Par(i+1, j+1));
                        else if(j>0 && j<7 && fin.Y == j-1)
                            lista.Add(new Par(i+1, j-1));
                    }
                }
            }
            return lista;
        }

        public List<Par> Qmov_torre(int i, int j)
        {
            List<Par> lista = new List<Par>();
            if(!juega){
                for(int a=j+1; a<8; a++){
                    if(Qtab[i][a]=="null")
                        lista.Add(new Par(i,a));
                    else {
                        if(Qtab[i][a][0]=='b')
                            lista.Add(new Par(i,a));
                        break;
                    }
                }
                for(int a=j-1; a>=0; a--){
                    if(Qtab[i][a]=="null")
                        lista.Add(new Par(i,a));
                    else {
                        if(Qtab[i][a][0]=='b')
                            lista.Add(new Par(i,a));
                        break;
                    }
                }
                for(int a=i+1; a<8; a++){
                    if(Qtab[a][j]=="null")
                        lista.Add(new Par(a,j));
                    else {
                        if(Qtab[a][j][0]=='b')
                            lista.Add(new Par(a,j));
                        break;
                    }
                }
                for(int a=i-1; a>=0; a--){
                    if(Qtab[a][j]=="null")
                        lista.Add(new Par(a,j));
                    else {
                        if(Qtab[a][j][0]=='b')
                            lista.Add(new Par(a,j));
                        break;
                    }
                }
            }
            else{
                for(int a=j+1; a<8; a++)
                    if(Qtab[i][a]=="null")
                        lista.Add(new Par(i,a));
                    else {
                        if(Qtab[i][a][0]=='w')
                            lista.Add(new Par(i,a));
                        break;
                    }
                for(int a=j-1; a>=0; a--)
                    if(Qtab[i][a]=="null")
                        lista.Add(new Par(i,a));
                    else {
                        if(Qtab[i][a][0]=='w')
                            lista.Add(new Par(i,a));
                        break;
                    }
                for(int a=i+1; a<8; a++)
                    if(Qtab[a][j]=="null")
                        lista.Add(new Par(a,j));
                    else {
                        if(Qtab[a][j][0]=='w')
                            lista.Add(new Par(a,j));
                        break;
                    }
                for(int a=i-1; a>=0; a--)
                    if(Qtab[a][j]=="null")
                        lista.Add(new Par(a,j));
                    else {
                        if(Qtab[a][j][0]=='w')
                            lista.Add(new Par(a,j));
                        break;
                    }
            }
            return lista;
        }

        public List<Par> Qmov_caballo(int i, int j)
        {
            List<Par> lista = new List<Par>();
            int a, b;
            if(!juega){
                a=i-2; b=j-1;
                if(a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i-1; b=j-2;
                if(a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i+1; b=j-2;
                if(a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i+2; b=j-1;
                if(a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i+2; b=j+1;
                if(a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i+1; b=j+2;
                if(a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i-1; b=j+2;
                if(a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i-2; b=j+1;
                if(a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
            }
            else{
                a=i-2; b=j-1;
                if(a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i-1; b=j-2;
                if(a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i+1; b=j-2;
                if(a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i+2; b=j-1;
                if(a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i+2; b=j+1;
                if(a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i+1; b=j+2;
                if(a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i-1; b=j+2;
                if(a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i-2; b=j+1;
                if(a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
            }
            return lista;
        }

        public List<Par> Qmov_alfil(int i, int j)
        {
            List<Par> lista = new List<Par>();
            if(!juega){
                for(int a=1; a < Math.Min(i+1,8-j); a++){
                    if(Qtab[i-a][j+a]=="null")
                        lista.Add(new Par(i-a,j+a));
                    else {
                        if(Qtab[i-a][j+a][0]=='b')
                            lista.Add(new Par(i-a,j+a));
                        break;
                    }
                }
                for(int a=1; a<Math.Min(8-i,8-j); a++){
                    if(Qtab[i+a][j+a]=="null")
                        lista.Add(new Par(i+a,j+a));
                    else {
                        if(Qtab[i+a][j+a][0]=='b')
                            lista.Add(new Par(i+a,j+a));
                        break;
                    }
                }
                for(int a=1; a<Math.Min(8-i,j+1); a++){
                    if(Qtab[i+a][j-a]=="null")
                        lista.Add(new Par(i+a,j-a));
                    else {
                        if(Qtab[i+a][j-a][0]=='b')
                            lista.Add(new Par(i+a,j-a));
                        break;
                    }
                }
                for(int a=1; a<Math.Min(i+1,j+1); a++){
                    if(Qtab[i-a][j-a]=="null")
                        lista.Add(new Par(i-a,j-a));
                    else {
                        if(Qtab[i-a][j-a][0]=='b')
                            lista.Add(new Par(i-a,j-a));
                        break;
                    }
                }
            }
            else{
                for(int a=1; a<Math.Min(i+1,8-j); a++){
                    if(Qtab[i-a][j+a]=="null")
                        lista.Add(new Par(i-a,j+a));
                    else {
                        if(Qtab[i-a][j+a][0]=='w')
                            lista.Add(new Par(i-a,j+a));
                        break;
                    }
                }
                for (int a = 1; a < Math.Min(8 - i, 8 - j); a++)
                {
                    if(Qtab[i+a][j+a]=="null")
                        lista.Add(new Par(i+a,j+a));
                    else {
                        if(Qtab[i+a][j+a][0]=='w')
                            lista.Add(new Par(i+a,j+a));
                        break;
                    }
                }
                for (int a = 1; a < Math.Min(8 - i, j + 1); a++)
                {
                    if(Qtab[i+a][j-a]=="null")
                        lista.Add(new Par(i+a,j-a));
                    else {
                        if(Qtab[i+a][j-a][0]=='w')
                            lista.Add(new Par(i+a,j-a));
                        break;
                    }
                }
                for (int a = 1; a < Math.Min(i + 1, j + 1); a++)
                {
                    if(Qtab[i-a][j-a]=="null")
                        lista.Add(new Par(i-a,j-a));
                    else {
                        if(Qtab[i-a][j-a][0]=='w')
                            lista.Add(new Par(i-a,j-a));
                        break;
                    }
                }
            }
            return lista;
        }

        public List<Par> Qmov_rey(int i, int j)
        {
            List<Par> lista = new List<Par>();
            int a, b;
            if (!juega)
            {
                a = i - 1; b = j - 1;
                if (a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b'))
                    lista.Add(new Par(a, b));
                a = i; b = j - 1;
                if (a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b'))
                    lista.Add(new Par(a, b));
                a = i + 1; b = j - 1;
                if (a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b'))
                    lista.Add(new Par(a, b));
                a = i + 1; b = j;
                if (a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b'))
                    lista.Add(new Par(a, b));
                a = i + 1; b = j + 1;
                if (a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b'))
                    lista.Add(new Par(a, b));
                a = i; b = j + 1;
                if (a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b'))
                    lista.Add(new Par(a, b));
                a = i - 1; b = j + 1;
                if (a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b'))
                    lista.Add(new Par(a, b));
                a = i - 1; b = j;
                if (a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b'))
                    lista.Add(new Par(a, b));

                if (!wkmov && !jaque("wk"))
                {
                    if (tab[i][j - 4] == "wr" && !wt1mov && Qtab[i][j - 1] == "null" && Qtab[i][j - 2] == "null" && Qtab[i][j - 3] == "null")
                        lista.Add(new Par(i, j - 2));
                    if (tab[i][j + 3] == "wr" && !wt2mov && Qtab[i][j + 1] == "null" && Qtab[i][j + 2] == "null")
                        lista.Add(new Par(i, j + 2));
                }
            }
            else
            {
                a = i - 1; b = j - 1;
                if (a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w'))
                    lista.Add(new Par(a, b));
                a = i; b = j - 1;
                if (a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w'))
                    lista.Add(new Par(a, b));
                a = i + 1; b = j - 1;
                if (a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w'))
                    lista.Add(new Par(a, b));
                a = i + 1; b = j;
                if (a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w'))
                    lista.Add(new Par(a, b));
                a = i + 1; b = j + 1;
                if (a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w'))
                    lista.Add(new Par(a, b));
                a = i; b = j + 1;
                if (a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w'))
                    lista.Add(new Par(a, b));
                a = i - 1; b = j + 1;
                if (a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w'))
                    lista.Add(new Par(a, b));
                a = i - 1; b = j;
                if (a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w'))
                    lista.Add(new Par(a, b));

                if (!bkmov && !jaque("bk"))
                {
                    if (tab[i][j - 4] == "br" && !bt1mov && Qtab[i][j - 1] == "null" && Qtab[i][j - 2] == "null" && Qtab[i][j - 3] == "null")
                        lista.Add(new Par(i, j - 2));
                    if (tab[i][j + 3] == "br" && !bt2mov && Qtab[i][j + 1] == "null" && Qtab[i][j + 2] == "null")
                        lista.Add(new Par(i, j + 2));
                }
            }
            return lista;
        }

        public List<Par> Qmov_peon_p(int i, int j)
        {
            List<Par> lista = new List<Par>();
            if(!juega && i>0) {
                if(j==0 && Qtab[i-1][j+1][0] == 'b')
                    lista.Add(new Par(i-1, j+1));
                else if(j == 7 && Qtab[i-1][j-1][0] == 'b')
                    lista.Add(new Par(i-1, j-1));
                else if(j>0 && j<7){
                    if(Qtab[i-1][j+1][0] == 'b')
                        lista.Add(new Par(i-1, j+1));
                    if(Qtab[i-1][j-1][0] == 'b')
                        lista.Add(new Par(i-1, j-1));
                }
            }
            else if(i<7){
                if(j==0 && Qtab[i+1][j+1][0] == 'w')
                    lista.Add(new Par(i+1, j+1));
                else if(j == 7 && Qtab[i+1][j-1][0] == 'w')
                    lista.Add(new Par(i+1, j-1));
                else if(j>0 && j<7){
                    if(Qtab[i+1][j+1][0] == 'w')
                        lista.Add(new Par(i+1, j+1));
                    if(Qtab[i+1][j-1][0] == 'w')
                        lista.Add(new Par(i+1, j-1));
                }
            }
            return lista;
        }

        public List<Par> Qmov_rey_p(int i, int j)
        {
            List<Par> lista = new List<Par>();
            int a, b;
            if(!juega){
                a=i-1; b=j-1;
                if(a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i; b=j-1;
                if(a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i+1; b=j-1;
                if(a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i+1; b=j;
                if(a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i+1; b=j+1;
                if(a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i; b=j+1;
                if(a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i-1; b=j+1;
                if(a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
                a=i-1; b=j;
                if(a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'b' ))
                    lista.Add(new Par(a,b));
            }
            else{
                a=i-1; b=j-1;
                if(a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i; b=j-1;
                if(a >= 0 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i+1; b=j-1;
                if(a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i+1; b=j;
                if(a < 8 && b >= 0 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i+1; b=j+1;
                if(a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i; b=j+1;
                if(a < 8 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i-1; b=j+1;
                if(a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
                a=i-1; b=j;
                if(a >= 0 && b < 8 && (Qtab[a][b] == "null" || Qtab[a][b][0] == 'w' ))
                    lista.Add(new Par(a,b));
            }
            return lista;
        }
    }

    class LChess{
        public Stack<chess> lchess;
        //list<QString> algebra;

        public LChess(){
            //lchess.Clear();
        }

        public void insert(chess a){
            //algebra.Add("jugada()");
            //lchess.Push(a);
        }

        public chess deshacer(){
            //algebra.pop_back();
            //if(lchess.Count() > 1)
            //    lchess.Pop();
            return lchess.Peek();
        }

        public void reset(){
            //algebra.clear();
            lchess.Clear();
        }
    }
}