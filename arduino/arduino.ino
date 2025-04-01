#define NUM_BUTTONS 20
#define NUM_OUTPUTS 4

const int buttonPins[NUM_BUTTONS] = {
  // Button, Down, Up, Left, Right
  44, 46, 48, 50, 52, // Player 1
  45, 47, 49, 51, 53, // Player 2
  6, 5, 4, 3, 2, // Player 3
  13, 12, 11, 10, 9 // Player 4
};
const int outputPins[NUM_OUTPUTS] = {22, 23, 24, 25}; // Outputs
bool lastInput[4] = {false, false, false, false};

void setup() {
    Serial.begin(115200);
    for (int i = 0; i < NUM_BUTTONS; i++) {
        pinMode(buttonPins[i], INPUT); // Active-low buttons
    }

    for (int i = 0; i < NUM_OUTPUTS; i++) {
        pinMode(outputPins[i], OUTPUT);
        digitalWrite(outputPins[i], LOW); // Default outputs to OFF
    }
}

void loop() {
    // Read all buttons and send their states
    for (int i = 0; i < NUM_BUTTONS; i++) {
        Serial.print(digitalRead(buttonPins[i]));
        Serial.print(i < NUM_BUTTONS - 1 ? "," : "\n"); // Format as CSV
    }

    // Read Unity command for outputs
    if (Serial.available() > 0) {
        String inputString = "";
        
        // Read all available lines, keeping only the most recent one
        while (Serial.available() > 0) {
            String tempLine = Serial.readStringUntil('\n');
            if (tempLine.length() > 0) {
                inputString = tempLine; // Keep overwriting with newer lines
            }
            // Small delay to allow more data to arrive if it's coming in chunks
            delay(1);
        }
        
        // Only process if we got a valid string
        if (inputString.length() > 0) {
            // Convert String to char array for strtok
            char buffer[64]; // Buffer to store incoming data
            inputString.toCharArray(buffer, sizeof(buffer));
            
            char* values[4]; // Array to store pointers to the split strings
            char* token;
            int stringCount = 0;

            // Get the first token
            token = strtok(buffer, ",");

            // Walk through other tokens
            while (token != NULL && stringCount < 4) {
                values[stringCount++] = token;
                token = strtok(NULL, ",");
            }
            
            // Process the values
            for (int i = 0; i < NUM_OUTPUTS && i < stringCount; i++) {
                // strcmp returns 0 when strings match, so logic needs to be inverted
                if (strcmp(values[i], "1") == 0) {
                    digitalWrite(outputPins[i], HIGH);
                } else {
                    digitalWrite(outputPins[i], LOW);
                }
            }
        }
    }

    delay(5); // Prevent spam
}
