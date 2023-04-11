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
import pandas as pd
from sklearn.preprocessing import LabelEncoder
from sklearn import preprocessing
import warnings

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
    def crop_image(img, tol=7):
        if img.ndim == 2:
            mask = img > tol
            return img[np.ix_(mask.any(1), mask.any(0))]
        elif img.ndim == 3:
            gray_img = cv2.cvtColor(img, cv2.COLOR_RGB2GRAY)
            mask = gray_img > tol
            check_shape = img[:, :, 0][np.ix_(mask.any(1), mask.any(0))].shape[0]
            if check_shape == 0:  # image is too dark so that we crop out everything,
                return img  # return original image
            else:
                img1 = img[:, :, 0][np.ix_(mask.any(1), mask.any(0))]
                img2 = img[:, :, 1][np.ix_(mask.any(1), mask.any(0))]
                img3 = img[:, :, 2][np.ix_(mask.any(1), mask.any(0))]
                img = np.stack([img1, img2, img3], axis=-1)

            return img
    def circle_crop(img):
        img = crop_image(img)

        height, width, depth = img.shape
        largest_side = np.max((height, width))
        img = cv2.resize(img, (largest_side, largest_side))

        height, width, depth = img.shape

        x = width // 2
        y = height // 2
        r = np.amin((x, y))

        circle_img = np.zeros((height, width), np.uint8)
        cv2.circle(circle_img, (x, y), int(r), 1, thickness=-1)
        img = cv2.bitwise_and(img, img, mask=circle_img)
        img = crop_image(img)

        return img
    if np.mean(image) < 10:
        return False
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    image = circle_crop(image)
    image = cv2.resize(image, (150, 150))
    image = cv2.addWeighted(image, 4, cv2.GaussianBlur(image, (0, 0), 10), -4, 128)
    return image



DR_Model = tf.keras.models.load_model("DR_Model_latest.h5")

DiabeticRetinopathy_Labels = {
    0: "No DR",
    1: "Mild",
    2: "Moderate",
    3: "Severe",
    4: "Proliferative DR",
}

with open('BrainStrokeModel.h5', 'rb') as file:
    BrainStroke_Model = pickle.load(file)

BrainStroke_Labels = {
    0: "No Stroke",
    1: "Stroke"
}

app = Flask(__name__)


@app.route("/DiabeticRetinopathy/", methods=["POST"])
def DiabeticRetinopathy_Prediction():
    try:
        #print(request)
        print(request.data)
        image_data = request.files['image'].read()
        # Load the image from binary data using PIL
        img = Image.open(io.BytesIO(image_data))
        # Convert the image to a NumPy array
        image_array = np.array(img)
        preprocessed_image = DiabeticRetinoapthy_Preprocessing(image_array)
        if preprocessed_image is False:
            return (
                jsonify({"status": False, "message": "Please enter a clear image"}),
                400,
            )
        content = np.expand_dims(preprocessed_image, axis=0)
        content = DR_Model.predict(content).round(3)
        prediction = np.argmax(content)
      
        response = {
            "status": True,
            "code": 200,
            "message": "Success",
            "data": str(DiabeticRetinopathy_Labels[int(prediction)]),
        }
        return jsonify(response), 200
     
    except Exception as e:
        print(e)
        return jsonify({"status": False, "message": f"Exception Message : {e}"}), 400

@app.route("/BrainStroke/", methods=["POST"])
def BrainStroke_Prediction():
    try:
        print(request.json)
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
        print(response)
        return jsonify(response), 200
        
    except Exception as e:
        print(e)
        return jsonify({"status": False, "message": f"Exception Message : {e}"}), 400
    

if __name__ == '__main__':
    app.run(port=8082)