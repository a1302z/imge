using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;



public class KaktusController : MonoBehaviour
{
    public bool isACTIVE;
    public string SerialPortName = "COM4";
    SerialPort stream;
    string receivedData = "EMPTY";
    public bool singletonPattern;
    public bool debug = false;
    public static KaktusController instance;
    public static List<KaktusController> instances = new List<KaktusController>();
    public bool[] buttonsPressed = new bool[6];
    public bool[] buttonsDown = new bool[6];
    //Slider Values in range [0, 1]
    public float[] sliderValues = new float[4];

    public bool[] lightsOn = new bool[4];
    private bool[] lightsChanged = new bool[4];
    public bool blinking = false;
    public int drehzahlMotor = 0;
    private int drehzahlChanged = 0;
    public bool useMotor;
    public int maxDrehzahl = 600;
    public int minDrehzahl = 150;

    public float micLoudness;
    /*
     * Cable is in front of you:
     * First Value: In Line with cable (back and forth)
     * Second Value: cross to controller (left and right)
     * Third Value: Up and down
     */
    public float[] accelerationXYZ = new float[3];

    /*
     * With bitmask:
     * */
    int[] bitmasks = {
                            Convert.ToInt32("000001000000", 2), //Button 1
                            Convert.ToInt32("000010000000",2), //Button 2
                            Convert.ToInt32("000100000000",2), //Button 3
                            Convert.ToInt32("001000000000",2), //Button 4
                            Convert.ToInt32("010000000000",2), //Button 5
                            Convert.ToInt32("100000000000",2), //Button 6
                        };

    void Awake()
    {
        if (singletonPattern)
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                GameObject.Destroy(this.gameObject);
            }
        }
        else if (!singletonPattern && !instances.Contains(this))
        {
            instances.Add(this);
        }
    }


    // Use this for initialization
    void Start()
    {
        if (isACTIVE)
        {
            stream = new SerialPort(SerialPortName, 115200);
            stream.Open(); //Open the Serial Stream
            reset();
            if (debug)
                Debug.Log("Serial Port opened.");

            for (int i = 0; i < buttonsPressed.Length; i++)
            {
                buttonsPressed[i] = false;
            }
            if (useMotor)
                drehzahlChanged = drehzahlMotor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isACTIVE)
        {
            readButtons();
            readSliders();
            readAcceleration();
            readMicrophone();
            if (!blinking)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (lightsOn[i] != lightsChanged[i])
                    {
                        sendToLights(i, lightsOn[i]);
                        lightsChanged[i] = lightsOn[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    lightsOn[i] = !lightsOn[i];
                    sendToLights(i, lightsOn[i]);
                }
            }
            if (useMotor)
                if (drehzahlMotor != drehzahlChanged)
                {
                    sendToMotor();
                    drehzahlChanged = drehzahlMotor;
                }
        }
    }

    public void reset()
    {
        for (int i = 0; i < 4; i++)
        {
            lightsOn[i] = false;
            sendToLights(i, false);
        }
        drehzahlMotor = 0;
        sendToMotor();
    }

    private void readSliders()
    {
        stream.Write("4");
        receivedData = stream.ReadLine();
        if (debug)
            Debug.Log(receivedData);
        string[] receivedDataArray = receivedData.Split(new Char[] { ' ' });
        for (int i = 0; i < sliderValues.Length; i++)
        {
            sliderValues[i] = Convert.ToInt32(receivedDataArray[i + 1], 16);
            sliderValues[i] /= (float)Convert.ToInt32("0FFF", 16);
            if (debug)
                Debug.Log("Slider " + i + " hat Wert " + sliderValues[i]);
        }
    }

    private void readButtons()
    {
        stream.Write("1");
        receivedData = stream.ReadLine();
        string binary;
        //sw6 - 1. bit von links
        //sw1 - 6. bit von links
        binary = Convert.ToString(Convert.ToInt32(receivedData, 16), 2).PadLeft(12, '0');
        int hexInt = Convert.ToInt32(receivedData, 16);
        //Debug.Log(binary);



        //Knöpfe auslesen
        for (int i = 0; i < buttonsPressed.Length; i++)
        {
            /*
             * Alternative 1: bits einzeln im Char Array prüfen
             */
            //bool buttonInput = readButtonInputCharArray(binary, i);
            /*
             * Alternative 2: mit Bitmasks
             */
            bool buttonInput = readButtonInputBitmask(hexInt, i);
            /*if (buttonInput)
                Debug.Log("Button " + i + " hat Bitmask: " + binary + "\n");*/
            buttonsDown[i] = buttonInput && !buttonsPressed[i];
            buttonsPressed[i] = buttonInput;
            //if (buttonsPressed[i]) Debug.Log("Button " + (i + 1) + " is pressed");
        }
    }

    private bool readButtonInputCharArray(string binary, int i)
    {
        bool buttonInput = binary.ToCharArray()[buttonsPressed.Length - i - 1].Equals('1');
        return buttonInput;
    }

    private bool readButtonInputBitmask(int hexInt, int i)
    {
        bool buttonInput = (hexInt & bitmasks[i]) != 0;
        return buttonInput;
    }

    private void sendToLights(int lightNr, bool on)
    {
        string s = "l ";
        s += lightNr + " ";
        s += (on) ? "1 " : "0 ";
        s += "\r\n";
        stream.Write(s);
        receivedData = stream.ReadLine();
    }

    private void sendToMotor()
    {
        int actualSpeed = drehzahlMotor;
        if (drehzahlMotor > 0 && drehzahlMotor < minDrehzahl)
        {
            actualSpeed = minDrehzahl;
        }
        stream.Write("m " + actualSpeed + " \r\n");

        receivedData = stream.ReadLine();
    }

    private void readAcceleration()
    {
        stream.Write("a");

        receivedData = stream.ReadLine();

        string[] splitReceivedData = receivedData.Split(' ');
        for (int i = 0; i < accelerationXYZ.Length; i++)
        {
            string hex = splitReceivedData[i + 1];
            accelerationXYZ[i] = (float)Convert.ToInt32(hex, 16);
            if (accelerationXYZ[i] > 128f) accelerationXYZ[i] -= 256f;
            //Debug.Log(i + ". Wert ist " + accelerationXYZ[i]);
            accelerationXYZ[i] /= 128f;
            if (debug)
                Debug.Log(i + ". Wert des Accelerometers ist " + accelerationXYZ[i]);
        }
    }

    public Vector3 getAccAsVector()
    {
        return new Vector3(accelerationXYZ[0], accelerationXYZ[1], accelerationXYZ[2]);
    }

    public void readMicrophone()
    {
        stream.Write("s");
        receivedData = stream.ReadLine();
        if (debug)
            Debug.Log(receivedData);
        receivedData = (string)receivedData.Split(' ').GetValue(1);
        micLoudness = float.Parse(receivedData, System.Globalization.CultureInfo.InvariantCulture);
        micLoudness /= 32000f;
        if (debug)
            Debug.Log(micLoudness);
    }
}
