//chatwindow including history and text input bar
import React from 'react';
import InputBar from './InputBar';

const ChatWindow = () => { 
    return (
        <div className="chatWindow">
            <div className="history">
                <p>History</p>
            </div>
            <InputBar />
        </div>
    );
};

export default ChatWindow;