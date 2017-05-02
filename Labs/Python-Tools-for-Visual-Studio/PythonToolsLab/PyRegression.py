'''
This script perfoms the basic process for applying a machine learning
algorithm to a dataset using Python libraries.

The four steps are:
   1. Download a dataset (using pandas)
   2. Process the numeric data (using numpy)
   3. Train and evaluate learners (using scikit-learn)
   4. Plot and compare results (using matplotlib)


The data is downloaded from URL, which is defined below. As is normal
for machine learning problems, the nature of the source data affects
the entire solution. When you change URL to refer to your own data, you
will need to review the data processing steps to ensure they remain
correct.

============
Example Data
============
The example is from http://mldata.org/repository/data/viewslug/stockvalues/
It contains stock prices and the values of three indices for each day
over a five year period. See the linked page for more details about
this data set.

This script uses regression learners to predict the stock price for
the second half of this period based on the values of the indices. This
is a naive approach, and a more robust method would use each prediction
as an input for the next, and would predict relative rather than
absolute values.
'''

# Remember to update the script for the new data when you change this URL
URL = "http://mldata.org/repository/data/download/csv/stockvalues/"

# This is the column of the sample data to predict.
# Try changing it to other integers between 1 and 155.
TARGET_COLUMN = 32

# Uncomment this call when using matplotlib to generate images
# rather than displaying interactive UI.
#import matplotlib
#matplotlib.use('Agg')

from pandas import read_table
import numpy as np
import matplotlib.pyplot as plt 
from sklearn.svm import SVR

try:
    # [OPTIONAL] Seaborn makes plots nicer
    import seaborn
except ImportError:
    pass

# =====================================================================

def download_data():
    '''
    Downloads the data for this script into a pandas DataFrame.
    '''

    frame = read_table(
        URL,

        encoding='latin-1',

        # Specify the separator in the data
        sep=',',            # comma separated values

        # Ignore spaces after the separator
        skipinitialspace=True,

        # Generate row labels from each row number
        index_col=None,

        # Generate column headers row from each column number
        header=None,
       
    )

    # Return the entire frame
    #return frame

    # Return a subset of the columns
    return frame[[156, 157, 158, TARGET_COLUMN]]


# =====================================================================


def get_features_and_labels(frame):
    '''
    Transforms and scales the input data and returns numpy arrays for
    training and testing inputs and targets.
    '''

    # Convert values to floats
    arr = np.array(frame, dtype=np.float)

    # Normalize the entire data set
    from sklearn.preprocessing import StandardScaler, MinMaxScaler
    arr = MinMaxScaler().fit_transform(arr)

    # Use the last column as the target value
    X, y = arr[:, :-1], arr[:, -1]
  
    # Use 50% of the data for training, but we will test against the
    # entire set
    
    # '''ADD LINES FOR CREATING TRAINING AND TEST SET'''
    
    # Normalize the attribute values to mean=0 and variance=1
    from sklearn.preprocessing import StandardScaler
    scaler = StandardScaler()
    
    # Fit the scaler based on the training data, then apply the same
    # scaling to both training and test sets.
    scaler.fit(X_train)
    X_train = scaler.transform(X_train)
    X_test = scaler.transform(X_test)

    # Return the training and test sets
    return X_train, X_test, y_train, y_test


# =====================================================================


def evaluate_learner(X_train, X_test, y_train, y_test):
    '''
    Run multiple times with different algorithms to get an idea of the
    relative performance of each configuration.

    Returns a sequence of tuples containing:
        (title, expected values, actual values)
    for each learner.
    '''
   
    # Train using a radial basis function
    

    # Train using a linear kernel
   

    # Train using a polynomial kernel
 


# =====================================================================


def plot(results):
    '''
    Create a plot comparing multiple learners.

    `results` is a list of tuples containing:
        (title, expected values, actual values)
    
    All the elements in results will be plotted.
    '''

    # Using subplots to display the results on the same X axis
    fig, plts = plt.subplots(nrows=len(results), figsize=(8, 8))
    fig.canvas.set_window_title('Predicting data from ' + URL)

    # Show each element in the plots returned from plt.subplots()
    for subplot, (title, y, y_pred) in zip(plts, results):
        # Configure each subplot to have no tick marks
        subplot.set_xticklabels(())
        subplot.set_yticklabels(())

        # Label the vertical axis
        subplot.set_ylabel('stock price')

        # Set the title for the subplot
        subplot.set_title(title)

        # Plot the actual data and the prediction
        subplot.plot(y, 'b', label='actual')
        subplot.plot(y_pred, 'r', label='predicted')
        
        # Shade the area between the predicted and the actual values
        subplot.fill_between(
            # Generate X values [0, 1, 2, ..., len(y)-2, len(y)-1]
            np.arange(0, len(y), 1),
            y,
            y_pred,
            color='r',
            alpha=0.2
        )

        # Mark the extent of the training data
        subplot.axvline(len(y) // 2, linestyle='--', color='0', alpha=0.2)

        # Include a legend in each subplot
        subplot.legend()

    # Let matplotlib handle the subplot layout
    fig.tight_layout()

    # ==================================
    # Display the plot in interactive UI
    plt.show()

    # Closing the figure allows matplotlib to release the memory used.
    plt.close()


# =====================================================================


if __name__ == '__main__':
    # Download the data set from URL
    print("Downloading data from {}".format(URL))
    frame = download_data()

    # Process data into feature and label arrays
    print("Processing {} samples with {} attributes".format(len(frame.index), len(frame.columns)))
    X_train, X_test, y_train, y_test = get_features_and_labels(frame)

    # Evaluate multiple regression learners on the data
    print("Evaluating regression learners")
    results = list(evaluate_learner(X_train, X_test, y_train, y_test))

    # Display the results
    print("Plotting the results")
    plot(results)
