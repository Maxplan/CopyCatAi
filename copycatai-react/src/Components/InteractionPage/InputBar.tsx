// Text input bar for sending requests to the API

import React from 'react';

const InputBar = () => {
    return (
    <div className="inputBar">
        <form>
            <input type="text" name="input" />
        </form>
        <button>send</button>
        </div>
)};

export default InputBar;