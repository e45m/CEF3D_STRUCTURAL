# Modelos de prueba CEF3D (kN, m) — acero E=204e6 kPa, sección 0.20x0.20 m (I=1.3333e-4 m4, EI=27200 kN·m2)

| Archivo | Descripción | Resultado de referencia |
|---|---|---|
| 01_voladizo_P | Voladizo L=3, P=10 kN en el extremo (Fz) | δ=PL³/3EI=3.309e-3 m; M=30 kN·m; V=10 kN |
| 02_viga_biapoyada_q | L=6, q=5 kN/m | δmax=5qL⁴/384EI=3.102e-3 m; Mmax=22.5; R=15 kN c/u |
| 03_viga_continua_2tramos_q | 2 tramos de 5 m, q=10 | R extremos=18.75, R central=62.5 kN; M apoyo central=31.25 kN·m |
| 04_empotrada_apoyada_q | L=4, q=10 | R apoyo=3qL/8=15; R empotr.=25; M empotr.=qL²/8=20; δmax≈qL⁴/185EI=5.09e-4 m |
| 05_portico_simple | Pórtico 6x4 m empotrado, H=20 kN y q=10 en viga | Equilibrio: ΣFx=-20, ΣFz=60 kN |
| 06_placa_voladizo | Placa 2x0.5 m, t=0.05, empotrada en x=0, carga total 1 kN en el borde libre | δ≈PL³/3EI≈2.5e-3 m (viga equivalente) |
| 07_placa_apoyada_carga_central | Placa 4x4 m, t=0.1, apoyada en el borde, P=100 kN al centro | w≈0.0116·P·a²/D≈9.9e-4 m (D=18681 kN·m) |

Todos con Analyzed=0 (sin resultados), para analizarlos en el programa y comparar.
Cargas distribuidas tipo global (Type=1, Fz); las placas con malla 4 nodos por elemento.
