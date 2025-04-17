#include <DHT.h>

const int pino_A0 = A0;  // Define o pino analógico A0
const int ledPin = 13;   // Pino do LED (ajuste conforme necessário)
bool ledEstado = false;  // Estado inicial do LED

// Define o pino onde o DHT11 está conectado
#define DHTPIN 2      

// Define o tipo de sensor
#define DHTTYPE DHT11  

// Inicializa o sensor DHT
DHT dht(DHTPIN, DHTTYPE);

void setup() {
  Serial.begin(9600);        // Inicia a comunicação serial
  pinMode(LED_BUILTIN, OUTPUT);  
  pinMode(ledPin, OUTPUT);    // Define pino do LED como saída
  digitalWrite(ledPin, LOW);  // Garante que o LED inicie apagado
  dht.begin();  
}

void loop() {
  // Verifica se há dados disponíveis na Serial
  if (Serial.available()) {
    String comando = Serial.readStringUntil('\n'); // Lê até nova linha
    comando.trim(); // Remove espaços extras

    if (comando == "L") {
      digitalWrite(ledPin, HIGH); // Liga o LED
    } else if (comando == "D") {
      digitalWrite(ledPin, LOW);  // Desliga o LED
    }
  }

  // Lê a umidade e a temperatura
  float h = dht.readHumidity();
  float t = dht.readTemperature();

  // Verifica se a leitura falhou
  if (isnan(t)) {
    Serial.println("Falha na leitura do sensor DHT11!");
    return;
  }

  // Lê o valor bruto do A0 (0 a 1023)
  int leitura_A0 = analogRead(pino_A0);
  float tensao_A0 = leitura_A0 * (5.0 / 1023.0); // Converte para tensão

  // Envia os dados para o C#
  Serial.print(t);  // Envia a temperatura
  Serial.print(","); // Delimitador para separar os dados
  Serial.print(h);  // Envia a umidade
  Serial.print(","); // Delimitador para separar os dados
  Serial.println(tensao_A0, 2);  // Envia a tensão do A0 (com 2 casas decimais)

  delay(1000);  // Espera 1 segundo entre leituras
}