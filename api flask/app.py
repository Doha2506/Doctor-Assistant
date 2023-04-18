#!/usr/bin/env python
# coding: utf-8

# In[ ]:


from flask import Flask, request, jsonify
import numpy as np
import requests
import tensorflow as tf
from io import BytesIO
from PIL import Image
import cv2
import os
import io
import pickle
import cv2
import pandas as pd
from sklearn.preprocessing import LabelEncoder
from sklearn import preprocessing
import warnings
import matplotlib.pyplot as plt

def BrainStroke_Preprocessing(raw_data):
    # Load the raw data into a pandas dataframe
    dataset= pd.read_csv('healthcare-dataset-stroke-data.csv')
    idx = len(dataset)
    dataset.loc[idx] = raw_data
    new_row_idx = dataset.iloc[-1].name
    dataset.drop("stroke",axis=1, inplace=True)  
    variables = dataset
    variables= variables.drop(columns= ['id'])
    variables['bmi'].fillna(value=variables['bmi'].mean(),inplace=True) 
    encode_x = LabelEncoder()
    variables['age'] = encode_x.fit_transform(variables['age'])
    variables = pd.concat( [variables, pd.get_dummies(variables['work_type'], prefix = 'work_type', drop_first = True)], axis = 1)
    variables.drop(['work_type'], axis = 1, inplace = True)
    variables = pd.concat( [variables, pd.get_dummies(variables['gender'], prefix = 'gender', drop_first = True)], axis = 1)
    variables.drop(['gender'], axis = 1, inplace = True)
    map_dict = {'Yes' : 1,'No' : 0}
    variables['ever_married'] = variables['ever_married'].map(map_dict);
    variables = pd.concat( [variables, pd.get_dummies(variables['Residence_type'], prefix = 'Residence_type', drop_first = True)], axis = 1)
    variables.drop(['Residence_type'], axis = 1, inplace = True)
    variables = pd.concat( [variables, pd.get_dummies(variables['smoking_status'], prefix = 'smoking_status', drop_first = True)], axis = 1)
    variables.drop(['smoking_status'], axis = 1, inplace = True)
    Q1 = variables['avg_glucose_level'].quantile(0.25)
    Q3 = variables['avg_glucose_level'].quantile(0.75)
    IQR = Q3 - Q1
    Min= Q1 - 1.5 * IQR
    Max= Q3 + 1.5 * IQR
    variables['avg_glucose_level'] = np.where(variables['avg_glucose_level'] <Min,19.2 ,variables['avg_glucose_level'])
    variables['avg_glucose_level'] = np.where(variables['avg_glucose_level'] >Max,115.16500000000002 ,variables['avg_glucose_level'])
    Q1 = variables['bmi'].quantile(0.25)
    Q3 = variables['bmi'].quantile(0.75)
    IQR = Q3 - Q1
    Min= Q1 - 1.5 * IQR
    Max= Q3 + 1.5 * IQR
    variables['bmi'] = np.where(variables['bmi'] <Min, 19.2,variables['bmi'])
    variables['bmi'] = np.where(variables['bmi'] >Max, 37.3,variables['bmi'])
    variables['age']= variables['age'].astype(str).astype('float64')
    variables['hypertension']= variables['hypertension'].astype(str).astype('float64')
    variables['heart_disease']= variables['heart_disease'].astype(str).astype('float64')
    variables['ever_married']= variables['ever_married'].astype(str).astype('float64')
    variables['work_type_Never_worked']= variables['work_type_Never_worked'].astype(str).astype('float64')
    variables['work_type_Private']= variables['work_type_Private'].astype(str).astype('float64')
    variables['work_type_Self-employed']= variables['work_type_Self-employed'].astype(str).astype('float64')
    variables['work_type_children']= variables['work_type_children'].astype(str).astype('float64')
    variables['gender_Male']= variables['gender_Male'].astype(str).astype('float64')
    variables['gender_Other']= variables['gender_Other'].astype(str).astype('float64')
    variables['Residence_type_Urban']= variables['Residence_type_Urban'].astype(str).astype('float64')
    variables['smoking_status_formerly smoked']= variables['smoking_status_formerly smoked'].astype(str).astype('float64')
    variables['smoking_status_never smoked']= variables['smoking_status_never smoked'].astype(str).astype('float64')
    variables['smoking_status_smokes']= variables['smoking_status_smokes'].astype(str).astype('float64')
    col_names = list(variables[['age', 'avg_glucose_level', 'bmi']])
    mm_scaler = preprocessing.MinMaxScaler()
    df_mm = mm_scaler.fit_transform(variables[['age', 'avg_glucose_level', 'bmi']])
    df_mm = pd.DataFrame(df_mm, columns=col_names)
    variables.drop(['age'], axis = 1, inplace = True)
    variables.drop(['avg_glucose_level'], axis = 1, inplace = True)
    variables.drop(['bmi'], axis = 1, inplace = True)
    variables = pd.concat( [df_mm, variables], axis = 1)
    return variables.loc[new_row_idx]


def DiabeticRetinoapthy_Preprocessing(image):
    def check_image_size(img):
        if img.shape[0] < 224 or img.shape[1] < 224:
            return False
        else:
            return True
    def check_retina_exist(img):
        img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        center_x, center_y = img.shape[1] // 2, img.shape[0] // 2
        radius = min(center_x, center_y) - 10  # Add a margin of 10 pixels
        mask = np.zeros(img.shape[:2], np.uint8)
        cv2.circle(mask, (center_x, center_y), radius, 255, -1)
        masked_image = cv2.bitwise_and(img, img, mask=mask)
        if cv2.countNonZero(masked_image) < 1000:
            return False
        else:
            return True
    check_size=check_image_size(image)
    check_retina=check_retina_exist(image)
    if (check_size==False):
        return "Image dimensions are too small for diabetic retinopathy classification."
    elif (check_retina==False):
        return "Image does not contain the retina."
    else:
        #gaussian = cv2.addWeighted(image, 4, cv2.GaussianBlur(image, (0,0), 10), -4, 128)
        #gaussian = cv2.resize(gaussian, (224, 224))
        return image




DR_Model = tf.keras.models.load_model("DiabeticRetinopathyModel.h5")

DiabeticRetinopathy_Labels = {
    0: "Mild",
    1: "Moderate",
    2: "NO_DR",
    3: "Proliferate_DR",
    4: "Severe",
}

with open('BrainStrokeModel.h5', 'rb') as file:
    BrainStroke_Model = pickle.load(file)

BrainStroke_Labels = {
    0: "No Stroke",
    1: "Stroke"
}

BrainTumor_Model = tf.keras.models.load_model("brain_model.h5")

BrainTumor_Labels = {
     0:'glioma_tumor',
      1:'no_tumor',
      2:'meningioma_tumor',
        3:'pituitary_tumor'   
}


BrainAlzhemir_Model = tf.keras.models.load_model("AD_Model.h5")


BrainAlzhemir_Labels = {
    2:'Non_Demented', 
    1:'Moderate_Demented', 
    3:'Very_Mild_Demented', 
    0:'Mild_Demented'
}

app = Flask(__name__)


@app.route("/DiabeticRetinopathy/", methods=["GET"])
def DiabeticRetinopathy_Prediction():
    try:
        file = request.files['image'].read()
        # Load the image from binary data using PIL
        img = Image.open(io.BytesIO(file))
        # Convert the image to a NumPy array
        image_array = np.array(img)
        preprocessed_image = DiabeticRetinoapthy_Preprocessing(image_array)
        if isinstance(preprocessed_image, str):
            Error_message=preprocessed_image
            return (
                jsonify({"status": False, "message": Error_message}),
                400,
            )
        prev_content = np.expand_dims(preprocessed_image, axis=0)
        content = DR_Model.predict(prev_content)
        prediction = np.argmax(content)
        response = {
            "status": True,
            "code": 200,
            "message": "Success",
            "data": str(DiabeticRetinopathy_Labels[int(prediction)]),
        }
        return jsonify(response), 200
    except Exception as e:
        return jsonify({"status": False, "message": f"Exception Message : {e}"}), 400

@app.route("/BrainTumor/", methods=["GET"])
def BrainTumor_Prediction():
    try:
        file = request.files['image'].read()
        # Load the image from binary data using PIL
        img = Image.open(io.BytesIO(file))
        # Convert the image to a NumPy array
        image_array = np.array(img)
        preprocessed_image = cv2.resize(image_array,(150, 150))
        prev_content = np.expand_dims(preprocessed_image, axis=0)
        content = BrainTumor_Model.predict(prev_content)
        prediction = np.argmax(content)
        response = {
            "status": True,
            "code": 200,
            "message": "Success",
            "data": str(BrainTumor_Labels[int(prediction)]),
        }
        return jsonify(response), 200
    except Exception as e:
        return jsonify({"status": False, "message": f"Exception Message : {e}"}), 400
    
@app.route("/BrainAlzhemir/", methods=["GET"])
def BrainAlzhemir_Prediction():
    try:
        file = request.files['image'].read()
        # Load the image from binary data using PIL
        img = Image.open(io.BytesIO(file))
        # Convert the image to a NumPy array
        image_array = np.array(img)
        preprocessed_image = cv2.resize(image_array,(128, 128))
        preprocessed_image = np.expand_dims(preprocessed_image, axis=-1)  # Add a new axis for color channel
        preprocessed_image = np.repeat(preprocessed_image, 3, axis=-1)  # Repeat the channel axis 3 times to get RGB image
        prev_content = np.expand_dims(preprocessed_image, axis=0)
        content = BrainAlzhemir_Model.predict(prev_content)
        prediction = np.argmax(content)
        response = {
            "status": True,
            "code": 200,
            "message": "Success",
            "data": str(BrainAlzhemir_Labels[int(prediction)]),
        }
        return jsonify(response), 200
    except Exception as e:
        return jsonify({"status": False, "message": f"Exception Message : {e}"}), 400
      

@app.route("/BrainStroke/", methods=["GET"])
def BrainStroke_Prediction():
    try:
        data = request.json
        preprocessed_data=BrainStroke_Preprocessing(data)
        new_data_2d = preprocessed_data.values.reshape(1, -1)
        prediction = BrainStroke_Model.predict(new_data_2d)
        prediction=int(prediction)
        response = {
            "status": True,
            "code": 200,
            "message": "Success",
            "data": str(BrainStroke_Labels[int(prediction)]),
        }
        return jsonify(response), 200
    except Exception as e:
        return jsonify({"status": False, "message": f"Exception Message : {e}"}), 400
    

if __name__ == '__main__':
    app.run(port=8082)


# In[ ]:




