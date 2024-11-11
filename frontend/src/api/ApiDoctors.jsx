// src/hooks/useFetch.js
import { useEffect, useState } from "react";
import axios from "axios";

const useFetch = (endpoint) => {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
  
    useEffect(() => {
      const fetchData = async () => {
        setLoading(true); // Start loading
  
        try {
          const response = await axios.get(endpoint);
          setData(response.data); // Set data from the response
        } catch (err) {
          // Log the error for debugging
          console.error("Error fetching data:", err);
          setError(err.message || "Something went wrong!"); // Set error message
        } finally {
          setLoading(false); // End loading
        }
      };
  
      fetchData();
    }, [endpoint]); // Fetch data when the endpoint changes
  
    return { data, loading, error }; // Return data, loading, and error state
  };
  

export default useFetch;
