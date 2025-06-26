✅ Exemplo: Lendo múltiplos registradores consecutivos
Suponha que você quer ler:

Nome					Endereço (exemplo)
Tensão do gerador		100
Corrente de excitação	101
Alarmes					102

Todos são ushort (16 bits). Então você pode fazer:
ushort[] Leituras = client.ReadHoldingRegisters<ushort>(SlaveAddress, 1, 7).ToArray();


Isso retorna um array com:
dados[0] → tensão
dados[1] → corrente
dados[2] → alarmes

🔄 Aplicando escala e exibindo

double tensao = dados[0] / 10.0;
double corrente = dados[1] / 100.0;
ushort alarme = dados[2];

🧠 Por que isso é melhor?
✅ Menos tráfego: 1 requisição em vez de 3.
✅ Mais rápido: economiza tempo de ida/volta (latência).
✅ Mais coeso: você pega todos os dados "no mesmo momento".

⚠️ Requisitos:
Os registradores precisam estar em endereços consecutivos.
Todos devem ser do mesmo tipo (ushort no caso).
Se for float (32 bits), cada valor ocupa 2 registradores, então a leitura precisa considerar isso.

Exemplo com float:
Se os valores forem float (32 bits = 2 registradores), como:

Nome			Endereço	Ocupa
Potência		200			2
Fator de Pot	202			2

Você pode fazer:
uint[] brutos = client.ReadHoldingRegisters<uint>(SlaveAddress, 200, 2);

float potencia = BitConverter.Int32BitsToSingle((int)brutos[0]);
float fatorPot = BitConverter.Int32BitsToSingle((int)brutos[1]);